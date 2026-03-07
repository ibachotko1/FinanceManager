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

        stage('Test') {
            steps {
                dir('FinanceManager') {
                    bat 'dotnet test FinanceManager/FinanceManager.Tests --configuration Release --no-build --logger trx'
                }
            }
        }

    post {
        success {
            echo 'Сборка успешна! Все тесты прошли.'
        }
        failure {
            echo 'Ошибка сборки или тестов. Проверьте последний коммит.'
        }
    }
}
