pipeline {
    agent any

    parameters {
        choice(name: 'ENV', choices: ['dev', 'staging', 'prod'], description: 'Select the environment')
    }

    environment {
        AWS_REGION = "us-east-1"
        S3_BUCKET = "terraform-state-bucket"
        DYNAMODB_TABLE = "terraform-lock"
    }

    stages {
        stage('Checkout Code') {
            steps {
                git branch: 'main', url: 'https://github.com/your-repo/terraform-infra.git'
            }
        }

        stage('Terraform Init') {
            steps {
                sh '''
                terraform init -backend-config="bucket=$S3_BUCKET" \
                               -backend-config="key=${ENV}/terraform.tfstate" \
                               -backend-config="region=$AWS_REGION" \
                               -backend-config="dynamodb_table=$DYNAMODB_TABLE"
                terraform workspace select ${ENV} || terraform workspace new ${ENV}
                '''
            }
        }

        stage('Terraform Plan') {
            steps {
                sh 'terraform plan -out=tfplan'
            }
        }

        stage('Terraform Apply') {
            steps {
                sh 'terraform apply -auto-approve tfplan'
            }
        }
    }
}
