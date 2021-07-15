pipeline {
    agent any

    environment {
        scannerHome = tool name : 'SonarScannerForMSBuild'
        registry = 'shweyasingh/sampleapi'
        properties = null
        docker_port = null
        username = 'shweyasingh'
    }

    options {
        timestamps()
        timeout(activity: true, time: 1, unit: 'HOURS')
        skipDefaultCheckout true
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
                script {
                    docker_port = 7100
                }
            }
        }

        stage('Restore NuGet Packages') {
            steps {
                echo 'NuGet restore step'
                bat 'dotnet restore'
            }
        }

        stage('Start SonarQube Analysis') {
            steps {
                echo 'Start sonarqube analysis step'
                withSonarQubeEnv('SonarQubeServer') {
                    bat "${scannerHome}\\SonarScanner.MSBuild.exe begin /k:SampleAPI /n:SampleAPI /v:1.0"
                }
            }
        }

        stage('Code build') {
            steps {
                echo 'Clean previous build'
                bat 'dotnet clean'

                echo 'Code build started'
                bat 'dotnet build -c Release -o "SampleAPI/app/build"'
            }
        }

        stage('Automated unit testing') {
            steps {
                echo 'Execute automated unit tests'
                bat 'dotnet test SampleAPI.Tests\\SampleAPI.Tests.csproj -l:trx;logFileName=SampleAPITestOutput.xml'
            }

            post {
                always {
                    echo 'Test report generation step'
                    xunit (
                        thresholds: [ skipped(failureThreshold: '0'), failed(failureThreshold: '0') ],
                        tools: [ MSTest(pattern: 'SampleAPI.Tests/TestResults/SampleAPITestOutput.xml') ]
                    )
                }
            }
        }

        stage('Stop SonarQube Analysis') {
            steps {
                echo 'Stop sonarqube analysis step'
                withSonarQubeEnv('SonarQubeServer') {
                    bat "${scannerHome}\\SonarScanner.MSBuild.exe end"
                }
            }
        }

        stage('Create Docker Image') {
            steps {
                echo 'Docker image step'
                bat 'dotnet publish -c Release'
                bat "docker build -t i_${username}_sampleapi --no-cache -f Dockerfile ."
            }
        }

        stage('Push Docker Image') {
            steps {
                echo 'Push docker image to docker hub step'
                bat "docker tag i_${username}_sampleapi ${registry}:${BUILD_NUMBER}"
                
                withDockerRegistry([credentialsId: 'DockerHub', url: ""]) {
                    bat "docker push ${registry}:${BUILD_NUMBER}"
                }
            }
        }

        stage('Docker Deployment') {
            steps {
                echo 'Docker deployment step'
                // bat 'docker ps -q --filter "name=SampleAPI"| findstr . && docker stop SampleAPI && docker rm -fv SampleAPI'
                bat "docker run --name SampleAPI -d -p 7100:80 ${registry}:${BUILD_NUMBER}"
            }
        }
    }
}
