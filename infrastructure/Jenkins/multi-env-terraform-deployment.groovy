pipeline {
    agent any

    parameters {
        choice(name: 'ENV', choices: ['dev', 'staging', 'prod'], description: 'Select the environment to deploy')
    }

    environment {
        AWS_REGION = "us-east-1"
        S3_BUCKET = "terraform-state-bucket"
        DYNAMODB_TABLE = "terraform-lock"
        TF_VERSION = "1.5.5"
    }

    stages {
        stage('Checkout Code') {
            steps {
                git branch: 'main', url: 'https://github.com/your-repo/terraform-infra.git'
            }
        }

        stage('Install Terraform') {
            steps {
                sh '''
                if ! terraform version | grep -q "${TF_VERSION}"; then
                  curl -O https://releases.hashicorp.com/terraform/${TF_VERSION}/terraform_${TF_VERSION}_linux_amd64.zip
                  unzip terraform_${TF_VERSION}_linux_amd64.zip
                  sudo mv terraform /usr/local/bin/
                fi
                '''
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
                sh 'terraform plan -var-file=config/${ENV}.tfvars -out=tfplan'
            }
        }

        stage('Approve Deployment') {
            when {
                expression { params.ENV == 'prod' } // Require approval for production
            }
            input {
                message "Apply Terraform changes to PRODUCTION?"
                ok "Yes, Deploy!"
            }
        }

        stage('Terraform Apply') {
            steps {
                sh 'terraform apply -auto-approve tfplan'
            }
        }
    }

    post {
        failure {
            echo "Terraform deployment failed! Rolling back..."
            sh 'terraform destroy -auto-approve -var-file=config/${ENV}.tfvars'
        }
        success {
            echo "Terraform applied successfully to ${ENV}!"
        }
    }
}
