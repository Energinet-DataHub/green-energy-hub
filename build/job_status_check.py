# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
import requests
import sys
import configargparse
import time
import logging

p = configargparse.ArgParser(prog='job_status_check.py', description='Check status of databricks jobs',
    formatter_class=configargparse.ArgumentDefaultsHelpFormatter
)
p.add('--job-run-ids', type=int, nargs="+", required=True,
    help='Job Run Id as output from job run step')

p.add('--token', type=str, required=True,
    help='Token to authenticate against databricks API')

p.add('--databricks-url', type=str, required=True,
    help='URL of Databricks Workspace')

p.add('--retries', type=int, required=True,
    help='Number of retries')

args = p.parse_args()

retries = args.retries
db_url = args.databricks_url

def databricks_request(path):
    headers = {'Authorization' : f'Bearer {args.token}'}
    return requests.get(f'{db_url}/api/2.0/{path}', headers=headers)

failed_jobs = []

for job_run_id in args.job_run_ids:
    
    attempts = 0
    run_id = job_run_id

    while attempts < retries:
        response = databricks_request(f'jobs/runs/get?run_id={run_id}')
        job_id = response.json()['job_id']
        logging.info(f'Checking status for job run: {run_id} with job id: {job_id }')
 
        while not ('result_state' in response.json()['state'] or response.json()['state']['life_cycle_state'] == 'RUNNING'):
            logging.info(f'Checking status for job run: {run_id} with job id: {job_id }')
            time.sleep(15)
            response = databricks_request(f'jobs/runs/get?run_id={run_id}')
        
        #Wait 30 second to see if did not fail
        if response.json()['state']['life_cycle_state'] == 'RUNNING':
            time.sleep(30)
            #check again. If still running we mark job as sucesfully started
            response = databricks_request(f'jobs/runs/get?run_id={run_id}')
            if response.json()['state']['life_cycle_state'] == 'RUNNING':
                break

        if response.json()['state']['result_state'] == 'SUCCESS':
            break
        else:
            attempts += 1
            logging.info(f'Job run {run_id} with job id: {job_id} failed. Retrying...')
            # Get new run id for particular job
            run_list_response = databricks_request(f'jobs/runs/list?job_id={job_id}&active_only=true&limit=1')
            run_id = run_list_response.json()['runs'][0]['run_id']
    
    if attempts == retries:
        failed_jobs.append(job_id)

if any(failed_jobs) :
    logging.info('Failed Job Ids: ' + ' '.join(map(str,failed_jobs)))   
    sys.exit(1)
else :
    logging.info('All jobs started sucesfully')
    sys.exit(0)

