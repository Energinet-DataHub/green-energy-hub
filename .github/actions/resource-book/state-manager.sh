#!/bin/bash

exec 3>&1

_AZURE_STORAGE_CONNECTION_STRING=$(echo "$AZURE_STORAGE_CONNECTION_STRING")
_AZURE_STORAGE_BLOB_NAME=$(echo "$AZURE_STORAGE_BLOB_NAME")
_AZURE_STORAGE_CONTAINER_NAME=$(echo "$AZURE_STORAGE_CONTAINER_NAME")
_RSM_ACTION=$(echo "$RSM_ACTION")
_RSM_BOOKER=$(echo "$RSM_BOOKER")
_RSM_RESOURCE=()
_RSM_GROUP=()
_SERVER="default"

#Local variables
NEW_FILE_NAME="newState"
LOG_FILE_NAME="rsm.log"
OUTPUT_FILE_NAME="rsm_output"
RESULT_FILE_NAME="rsm_result"
readInputResult=""
numOfIterations=60 #max 5 mins to aquire lease, 60 mins to wait until resources will be released

function log()
{
    #Add trainilg spaces to function name to fit 20 symbols size
    totalSize="                    "
    funcName=$1
    funcName="${funcName:0:20}${totalSize:0:$((20 - ${#funcName}))}"
    echo "$(date '+%d/%m/%Y %H:%M:%S')  $funcName: $2" 1>&3
    echo "$(date '+%d/%m/%Y %H:%M:%S')  $funcName: $2" 1>>$LOG_FILE_NAME
}

function setOutput()
{
    export $1=$2

    if [ $_SERVER == "github" ]; then
        # Export to GITHUB env
        echo $1=$2 >> $GITHUB_ENV
        # Set GitHub Step output
        echo "::set-output name=$1::$2" 1>&3
    fi
}

function readInput()
{
    while [ $# -gt 0 ]; do
    case "$1" in
        -s|-cs|--connection-string)
        _AZURE_STORAGE_CONNECTION_STRING=$(echo "$2" | xargs)
        ;;
        -b|-bn|--blob-name)
        _AZURE_STORAGE_BLOB_NAME=$(echo "$2" | xargs)
        ;;
        -c|-cn|--container-name)
        _AZURE_STORAGE_CONTAINER_NAME=$(echo "$2" | xargs)
        ;;
        -a|-ac|--action)
        _RSM_ACTION=($(echo "$2" | xargs))
        ;;
        -g|-gr|--group)
        OLDIFS=$IFS
        IFS=',' ;for rsmGroupItem in $2; do _RSM_GROUP+=($(echo "$rsmGroupItem" | xargs)); done
        IFS=$OLDIFS
        ;;
        -r|-rs|--resource)
        OLDIFS=$IFS
        IFS=',' ;for rsmResourceItem in $2; do _RSM_RESOURCE+=($(echo "$rsmResourceItem" | xargs)); done
        IFS=$OLDIFS
        ;;
        -b|-bk|--booker)
        _RSM_BOOKER=$(echo "$2" | xargs)
        ;;
        -s|-sv|--server)
        _SERVER=$(echo "$2" | xargs)
        ;;
        -h|-hp|--help)
        #Run help function here
        ;;
        *)
        log "readInput()" "Wrong argument \"$1\""
        exit 1
    esac
    shift
    shift
    done

    #-------------------------------------Validate parameters
    #Check "ACTION" has valid input
    _RSM_ACTION=$(echo "$_RSM_ACTION" | tr '[:upper:]' '[:lower:]')
    if [ $_RSM_ACTION != "book" ] && [ $_RSM_ACTION != "release" ]; then
        log "readInput()"  "Invalid parameter value. Parameter = \"ACTION\", Value = \"$_RSM_ACTION\". Should be \"book\" or \"release\""
        exit 1
    fi

    #Check "AZURE_STORAGE_CONNECTION_STRING" is not empty
    if [ -z "$_AZURE_STORAGE_CONNECTION_STRING" ]; then
        log "readInput()"  "Invalid parameter value. Parameter \"connection-string\" should not be empty"
        exit 1
    fi

    #Check "AZURE_STORAGE_BLOB_NAME" is not empty
    if [ -z "$_AZURE_STORAGE_BLOB_NAME" ]; then
        log "readInput()"  "Invalid parameter value. Parameter \"blob-name\" should not be empty"
        exit 1
    fi

    #Check "AZURE_STORAGE_CONTAINER_NAME" is not empty
    if [ -z "$_AZURE_STORAGE_CONTAINER_NAME" ]; then
        log "readInput()"  "Invalid parameter value. Parameter \"container-name\" should not be empty"
        exit 1
    fi

    #Nornalize "SERVER" parameter (transform to lower case)
    if [ ! -z "$_SERVER" ]; then
        _SERVER=$(echo $_SERVER | tr '[:upper:]' '[:lower:]')
    fi

    #Check required parameters for "book" action
    if [ $_RSM_ACTION == "book" ]; then
        if [ -z "$_RSM_GROUP" ]; then
            log "readInput()"  "Invalid parameter value. Parameter \"group\" should not be empty"
            exit 1
        else
            #Check for dublicates
            distinctCount=$(printf '%s\n' "${_RSM_GROUP[@]}"|awk '!($0 in checked){checked[$0];c++} END {print c}')
            if [ $distinctCount != ${#_RSM_GROUP[@]} ]; then
                log "readInput()" "Invalid parameter value. Parameter \"group\" has dublicates."
                exit 1
            fi
        fi
        if [ ! -z "$_RSM_RESOURCE" ]; then
            log "readInput()"  "A \"resource\" parameter is set, but will be ignored for \"book\" action"
        fi
    fi

    #Check required parameters for "release" action
    if [ $_RSM_ACTION == "release" ]; then
        if [ -z "$_RSM_RESOURCE" ]; then
            log "readInput()"  "Invalid parameter value. Parameter \"resource\" should not be empty"
            exit 1
        else
            #Check for dublicates
            distinctCount=$(printf '%s\n' "${_RSM_RESOURCE[@]}"|awk '!($0 in checked){checked[$0];c++} END {print c}')
            if [ $distinctCount != ${#_RSM_RESOURCE[@]} ]; then
                log "readInput()" "Invalid parameter value. Parameter \"resource\" has dublicates."
                exit 1
            fi
        fi
        if [ ! -z "$_RSM_GROUP" ]; then
            log "readInput()"  "A \"group\" parameter is set, but will be ignored for \"release\" action"
        fi
    fi

    # Print variables
    # log "readInput()"  "AZURE_STORAGE_CONNECTION_STRING: \"$_AZURE_STORAGE_CONNECTION_STRING\""
    log "readInput()"  "AZURE_STORAGE_BLOB_NAME:         \"$_AZURE_STORAGE_BLOB_NAME\""
    log "readInput()"  "AZURE_STORAGE_CONTAINER_NAME:    \"$_AZURE_STORAGE_CONTAINER_NAME\""
    log "readInput()"  "ACTION:                          \"$_RSM_ACTION\""
    log "readInput()"  "BOOKER:                          \"$_RSM_BOOKER\""
    for g in ${_RSM_GROUP[@]}; do
        log "readInput()"  "GROUP:                           \"$g\""
    done
    for r in ${_RSM_RESOURCE[@]}; do
        log "readInput()"  "RESOURCE:                        \"$r\""
    done

    readInputResult="True"

} 2>>$LOG_FILE_NAME

function acquireLease()
{
    IsBlobExists=$(az storage blob exists       --container-name $_AZURE_STORAGE_CONTAINER_NAME \
                                                --name $_AZURE_STORAGE_BLOB_NAME \
                                                --connection-string $_AZURE_STORAGE_CONNECTION_STRING \
                                                --auth-mode key \
                                                --output tsv)
    #Check if target blob exists
    if [ $IsBlobExists == $'False' ] 
    then
        log "acquireLease()" "There is no blob \"$_AZURE_STORAGE_BLOB_NAME\" in container \"$_AZURE_STORAGE_CONTAINER_NAME\" in given storage account. Have you made initial setup?"
        echo "False"
        return
    fi

    #Runnig a loop (retry logic) to aquire 60 sec lease on blob
    for (( i=0; i<$numOfIterations; i++ ))
    do
        log "acquireLease()" "$i attempt to aquire lease"
        #Try to aquire lease
        aquireLeaseResult=$(az storage blob lease acquire   --container-name $_AZURE_STORAGE_CONTAINER_NAME \
                                                            --blob-name $_AZURE_STORAGE_BLOB_NAME \
                                                            --proposed-lease-id $leaseId \
                                                            --lease-duration 60 \
                                                            --connection-string $_AZURE_STORAGE_CONNECTION_STRING \
                                                            --auth-mode key)
        #Validate lease aquired
        if [ "$aquireLeaseResult" == \"$leaseId\" ]
        then
            echo "True"
            return
        fi

        sleep 5
    done

    echo "False"
    return
} 2>>$LOG_FILE_NAME

function releaseLease()
{
    #Releasing an lease, as we finished everything we need.
    releaseResult=$(az storage blob lease release   --container-name $_AZURE_STORAGE_CONTAINER_NAME \
                                                    --blob-name $_AZURE_STORAGE_BLOB_NAME \
                                                    --lease-id $leaseId \
                                                    --connection-string $_AZURE_STORAGE_CONNECTION_STRING \
                                                    --auth-mode key)
    log "releaseLease()" "Blob lease has been released."

    echo "True"
    return
} 2>>$LOG_FILE_NAME

function downloadBlob()
{
    #Downloading blob into file locally
    downloadResult=$(az storage blob download   --container-name $_AZURE_STORAGE_CONTAINER_NAME \
                                                --name $_AZURE_STORAGE_BLOB_NAME \
                                                --file $_AZURE_STORAGE_BLOB_NAME \
                                                --connection-string $_AZURE_STORAGE_CONNECTION_STRING \
                                                --auth-mode key \
                                                --output tsv)
    #Validate download result
    if [ -z "$downloadResult" ]
    then
        log "downloadBlob()" "Failed to download blob \"$_AZURE_STORAGE_BLOB_NAME\" from container \"$_AZURE_STORAGE_CONTAINER_NAME\" from given storage account."
        echo "False"
    else
        echo "True"
    fi
} 2>>$LOG_FILE_NAME

function bookResource()
{
    local bookedGroupArr=()
    #Read whole content of the file
    fileContent=$(cat $_AZURE_STORAGE_BLOB_NAME)
    #Check file is not empty
    if [ ! -z "$fileContent" ]; then
        #Read file content line by line
        while IFS= read -r line; do
            #--------Analyze line---------
            #Getting a group identifier of this line (splitting the line by "|" and taking 3d element)
            #Also, removing '\r' in case this file made in Windows
            group=$(echo $line | awk -F '|' '{print $3}' | tr -d '\r')
            #Check that result is not empty
            if [ ! -z "$group" ]; then
                #This iterator will be used to check, that current line doesn't has a required resource,
                #so we need to add it back to result file without changes
                local i=0
                local booked="False"
                # Iterating over array, provided in "resource" input
                for _RSM_GROUP_ITEM in ${_RSM_GROUP[@]}; do
                    #Check that current group ($_RSM_GROUP_ITEM) is not yet processed
                    #if it is, set booked="True", so than we will ignore this line
                    booked="False"
                    for bookedGroupItem in ${bookedGroupArr[@]}; do
                        if [ $_RSM_GROUP_ITEM == $(echo $bookedGroupItem | awk -F '|' '{print $1}') ]; then
                            booked="True"
                        fi
                    done
                    #Checking that group identifier matches one, that was set as parameter for this script
                    #If it doesn't, just ignoring processing this line
                    if [ $_RSM_GROUP_ITEM == $group ]; then
                        #Getting a state from this line (splitting the line by "|" and taking 2d element)
                        state=$(echo $line | awk -F '|' '{print $2}' | tr '[:upper:]' '[:lower:]')
                        #Check, that state is not empty
                        if [ ! -z "$state" ]; then
                            #If current line not booked yet, and we didn't book prevoius lines
                            if [ $state == "free" ] && [ $booked == "False" ]; then
                                #Getting a service name from this line (splitting the line by "|" and taking 1st element)
                                resource=$(echo $line | awk -F '|' '{print $1}' | tr '[:upper:]' '[:lower:]')
                                log "bookResource()" "Booking service \"$resource\" in group \"$group\""
                                # If booker identifier provided, add line with this identifier to result file
                                if [ ! -z "$_RSM_BOOKER" ]; then
                                    echo $(echo "$resource|booked|$_RSM_GROUP_ITEM|$(date +%s%3N)|$_RSM_BOOKER") >>$NEW_FILE_NAME
                                else
                                    echo $(echo "$resource|booked|$_RSM_GROUP_ITEM|$(date +%s%3N)") >>$NEW_FILE_NAME
                                fi
                                # Add this group and resource to temporary array, so we will ignore it on next run
                                bookedGroupArr+=($(echo "$_RSM_GROUP_ITEM|$resource"))
                            else
                                i=$(($i + 1))
                            fi
                        else
                            # If group identifier is empty, file is broken. Report it in log.
                            log "bookResource()" "File \"$_AZURE_STORAGE_BLOB_NAME\" is broken. State is empty."
                            exit 1
                            return
                        fi
                    else
                        i=$(($i + 1))
                    fi
                done
                #If this resource not matched one, defined in parameters, just add it back to file without changes
                if [ $i == ${#_RSM_GROUP[@]} ]; then
                    echo $(echo "$line" | tr -d '\r') >>$NEW_FILE_NAME
                fi
            else
                # If group identifier is empty, file is broken. Report it in log.
                log "bookResource()" "File \"$_AZURE_STORAGE_BLOB_NAME\" is broken. Group identifier is empty."
                exit 1
                return
            fi
            #-----------------------------
        done <<< "$fileContent"
        #Final check: make sure, that number of groups processed equal to number of group, provided as input
        if [ ${#bookedGroupArr[@]} == ${#_RSM_GROUP[@]} ]; then
            #Prepare output in the same order as group parametes has been provided
            OLDIFS=$IFS
            IFS=$'\n'
            local outputStr=""
            for _RSM_GROUP_ITEM in ${_RSM_GROUP[@]}; do
                for _BOOKED_ITEM in ${bookedGroupArr[@]}; do
                    if [ $_RSM_GROUP_ITEM == $(echo $_BOOKED_ITEM | awk -F '|' '{print $1}') ]; then
                        #extracting resoure
                        resource=$(echo $_BOOKED_ITEM | awk -F '|' '{print $2}')
                        # Add booked resource to output file
                        echo $resource >> $OUTPUT_FILE_NAME
                        # Export to env variable
                        setOutput $(echo "RB_OUT_$_RSM_GROUP_ITEM") $resource
                        # Add to output array
                        outputStr="$outputStr$resource,"
                    fi
                done
            done
            setOutput "RB_OUTPUT" "${outputStr::-1}"
            IFS=$OLDIFS
            echo "True"
        else
            echo "False"
        fi
    else
        #If file is empty, returning with "False" result
        log "bookResource()" "File \"$_AZURE_STORAGE_BLOB_NAME\" is empty."
        exit 1
        return
    fi
} 2>>$LOG_FILE_NAME

function releaseResource()
{
    released="False"
    #Read whole content of the file
    fileContent=$(cat $_AZURE_STORAGE_BLOB_NAME)
    #Check file not empty
    if [ ! -z "$fileContent" ]; then
        #Read file content line by line
        while IFS= read -r line; do
            #--------Analyze line---------
            #Getting a resource of this line (splitting the line by "|" and taking 3d element)
            #Also, removing '\r' in case this file made in Windows
            resource=$(echo $line | awk -F '|' '{print $1}' | tr -d '\r')
            #Check that result is not empty
            if [ ! -z "$resource" ]; then
                #This iterator will be used to check, that current line doesn't has a required resource,
                #so we need to add it back to result file without changes
                local i=0
                # Iterating over array, provided in "resource" input
                for _RSM_RESOURCE_ITEM in ${_RSM_RESOURCE[@]}; do
                    #Checking that resource matches one, that was set as parameter for this script
                    #If it doesn't, just ignoring processing this line
                    if [ $_RSM_RESOURCE_ITEM == $resource ]; then
                        #Getting a group identifier of this line (splitting the line by "|" and taking 3d element)
                        #Also, removing '\r' in case this file made in Windows
                        group=$(echo $line | awk -F '|' '{print $3}' | tr -d '\r')
                        #Check, if group is empty
                        if [ ! -z "$group" ]; then
                            log "releaseResource()" "Releasing service \"$resource\" in group \"$group\""
                            released="True"
                            echo $(echo "$resource|free|$group|$(date +%s%3N)") >>$NEW_FILE_NAME
                        else
                            # If group identifier is empty, file is broken. Report it in log.
                            log "releaseResource()" "File \"$_AZURE_STORAGE_BLOB_NAME\" is broken. Group identifier is empty."
                            exit 1
                            return
                        fi
                    else
                        i=$(($i + 1))
                    fi
                done
                #If this resource not matched one, defined in parameters, just add it back to file without changes
                if [ $i == ${#_RSM_RESOURCE[@]} ]; then
                    echo $(echo "$line" | tr -d '\r') >>$NEW_FILE_NAME
                fi
            else
                # If group identifier is empty, file is broken. Report it in log.
                log "releaseResource()" "File \"$_AZURE_STORAGE_BLOB_NAME\" is broken. Resource identifier is empty."
                exit 1
                return
            fi
            #-----------------------------
        done <<< "$fileContent"
        echo $released
    else
        #If file is empty, returning with "False" result
        log "releaseResource()" "File \"$_AZURE_STORAGE_BLOB_NAME\" is empty."
        exit 1
        return
    fi
} 2>>$LOG_FILE_NAME

function uploadAndRemove()
{
    uploadResult=$(az storage blob upload --container-name $_AZURE_STORAGE_CONTAINER_NAME \
                                            --name $_AZURE_STORAGE_BLOB_NAME \
                                            --file $NEW_FILE_NAME \
                                            --lease-id $leaseId \
                                            --connection-string $_AZURE_STORAGE_CONNECTION_STRING \
                                            --auth-mode key)

    #Validate upload result
    if [ -z "$uploadResult" ]
    then
        log "uploadAndRemove()" "Blob \"$_AZURE_STORAGE_BLOB_NAME\" from file \"$NEW_FILE_NAME\" to container \"$_AZURE_STORAGE_CONTAINER_NAME\" in given storage account."
        echo "False"
    else
        log "uploadAndRemove()" "Blob has been uploaded."
        releaseResult=$(releaseLease)
        echo "True"
    fi
} 2>>$LOG_FILE_NAME


function main()
{
    local executionResult="False"

    leaseId=$(cat /proc/sys/kernel/random/uuid)
    if [ -z $leaseId ]; then
        log "main()" "Failed to generate leaseId. See log file: $LOG_FILE_NAME"
        exit 1
    fi

    log "main()" "LEASE_ID: $leaseId"

    for (( i=0; i<$numOfIterations; i++ ))
    do
        #Aquire lease on blob, that stores a state
        aquireLeaseResult=$(acquireLease)
        if [ $aquireLeaseResult == "True" ]; then
            #Download blob
            downloadResult=$(downloadBlob)
            if [ $downloadResult == "True" ]; then
                #Depending on action, book or release resource
                processResult=""
                if [ $_RSM_ACTION == "book" ]; then
                    processResult=$(bookResource)
                elif [ $_RSM_ACTION == "release" ]; then
                    processResult=$(releaseResource)
                fi
                #Upload new state file in blob
                if [ $processResult == "True" ]; then
                    uploadResult=$(uploadAndRemove)
                    if [ $uploadResult == "True" ]; then
                        #Exit loop and report success
                        executionResult="True"
                        log "main()" "Success!"
                        break
                    else
                        #Exit loop and report error
                        log "main()" "Failed to upload blob. See log file: $LOG_FILE_NAME"
                        break
                    fi
                elif [ $processResult == "False" ]; then
                    #Looks like all resources are booked. Just wait while one of them will be released
                    log "main()" "Looks like all resources are booked. Waiting 60 sec and then retry..."
                    rm -f $NEW_FILE_NAME
                    releaseResult=$(releaseLease)
                else
                    #Exit loop and report error
                    log "main()" "Failed to book resource. See log file: $LOG_FILE_NAME"
                    break
                fi
            else
                #Exit loop and report error
                log "main()" "Failed to download blob. See log file: $LOG_FILE_NAME"
                break
            fi
        else
            #Exit loop and report error
            log "main()" "Failed to aquire lease. See log file: $LOG_FILE_NAME"
            break
        fi

        sleep 60
    done

    echo $executionResult
    echo $executionResult >>$RESULT_FILE_NAME
    rm -f $NEW_FILE_NAME
    rm -f $_AZURE_STORAGE_BLOB_NAME
} 2>>$LOG_FILE_NAME

#Remove files from previous run (just in case...)
rm -f $_AZURE_STORAGE_BLOB_NAME
rm -f $NEW_FILE_NAME
rm -f $LOG_FILE_NAME
rm -f $RESULT_FILE_NAME
rm -f $OUTPUT_FILE_NAME

readInput "$@"
if [ ! -z $readInputResult ] && [ $readInputResult == "True" ]; then
    #Run main logic
    result=$(main)
    if [ $result == "True" ]; then
        setOutput "RB_RESULT" "True"
        exit 0
    else
        setOutput "RB_RESULT" "False"
        exit 1
    fi
else
    echo "Input error. See log file: $LOG_FILE_NAME"
    echo "False" >>$RESULT_FILE_NAME
    setOutput "RB_RESULT" "False"
    exit 1
fi
