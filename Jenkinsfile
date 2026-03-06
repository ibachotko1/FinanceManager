pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    }

    stages {
        stage('Restore') {
            steps {
                dir('FinanceManager') {
                    bat 'dotnet restore FinanceManager.slnx'
                }
            }
        }

        stage('Build') {
            steps {
                dir('FinanceManager') {
                    bat 'dotnet build FinanceManager.slnx --configuration Release --no-restore'
                }
            }
        }
    }

    post {
        success {
            echo 'Сборка успешна!'
        }
        failure {
            echo 'Ошибка сборки. Проверьте последний коммит.'
        }
    }
}
