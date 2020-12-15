#!/bin/bash -l
# Run all jobs in the comma separated array of job ids
res=""
IFS=',' read -ra IDS <<<  "$1"
for id in "${IDS[@]}" 
do
  json=$(databricks jobs run-now --job-id $id)
  run_id=$(echo $json | python -c "import sys, json; print(json.load(sys.stdin)['run_id'])")
  res+="${run_id} "
done

echo $res