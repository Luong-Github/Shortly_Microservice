import json
import boto3
import os
import random
import string
import logging

secretsmanager = boto3.client('secretsmanager')
rds = boto3.client('rds')

logger = logging.getLogger()
logger.setLevel(logging.INFO)

def generate_password(length=16):
    """Generate a random secure password."""
    characters = string.ascii_letters + string.digits + "!@#$%^&*()"
    return ''.join(random.choice(characters) for _ in range(length))

def get_secret(secret_arn):
    """Retrieve the current secret from AWS Secrets Manager."""
    response = secretsmanager.get_secret_value(SecretId=secret_arn)
    return json.loads(response['SecretString'])

def update_secret(secret_arn, new_password):
    """Update the secret value in AWS Secrets Manager."""
    current_secret = get_secret(secret_arn)
    current_secret['password'] = new_password
    secretsmanager.put_secret_value(
        SecretId=secret_arn,
        SecretString=json.dumps(current_secret)
    )

def modify_rds_password(db_instance_id, username, new_password):
    """Update the RDS database with the new password."""
    response = rds.modify_db_instance(
        DBInstanceIdentifier=db_instance_id,
        MasterUserPassword=new_password,
        ApplyImmediately=True
    )
    return response

def lambda_handler(event, context):
    """Handle rotation events triggered by AWS Secrets Manager."""
    secret_arn = event['SecretId']
    step = event['Step']
    
    logger.info(f"Handling rotation step: {step} for secret: {secret_arn}")

    secret_data = get_secret(secret_arn)
    db_instance_id = secret_data['dbInstanceIdentifier']
    username = secret_data['username']

    if step == 'createSecret':
        new_password = generate_password()
        update_secret(secret_arn, new_password)
        logger.info("New password generated and stored in Secrets Manager.")

    elif step == 'setSecret':
        new_secret = get_secret(secret_arn)
        modify_rds_password(db_instance_id, username, new_secret['password'])
        logger.info("New password applied to RDS instance.")

    elif step == 'testSecret':
        logger.info("Testing new password... (Skipping actual test in this demo)")

    elif step == 'finishSecret':
        logger.info("Rotation complete. Marking new password as current.")

    return {"status": "success"}
