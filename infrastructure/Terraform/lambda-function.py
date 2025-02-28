import boto3
import gzip
import json
import os
import tempfile

s3 = boto3.client('s3')
logs_client = boto3.client('logs')

LOG_GROUP_NAME = "/aws/alb/logs"

def lambda_handler(event, context):
    for record in event['Records']:
        bucket_name = record['s3']['bucket']['name']
        object_key = record['s3']['object']['key']

        # Download log file
        temp_file = tempfile.mktemp()
        s3.download_file(bucket_name, object_key, temp_file)

        # Read and process log file
        with gzip.open(temp_file, 'rt') as log_file:
            logs = log_file.readlines()

        # Send logs to CloudWatch
        for log in logs:
            log_event = {
                'logGroupName': LOG_GROUP_NAME,
                'logStreamName': "alb-log-stream",
                'logEvents': [
                    {
                        'timestamp': int(record['eventTime']),
                        'message': log
                    }
                ]
            }
            logs_client.put_log_events(**log_event)

    return {
        'statusCode': 200,
        'body': json.dumps('Logs processed and sent to CloudWatch!')
    }
