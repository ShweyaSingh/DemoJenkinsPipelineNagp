pipeline {
    agent any

    environment {
        scannerHome = tool name : 'sonar_scanner_dotnet'
        registry = 'shweyasingh/sampleapi'
        docker_port = 7100
        username = 'shweta03'
        container_id = "${bat(script:'docker ps -q --filter name=c-shweta03-master', returnStdout: true).trim().readLines().drop(1).join("")}"
    }

    options {
        timestamps()
        timeout(activity: true, time: 1, unit: 'HOURS')
        skipDefaultCheckout true
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Code checkout step'
                checkout scm
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
                withSonarQubeEnv('Test_Sonar') {
                    bat "${scannerHome}\\SonarScanner.MSBuild.exe begin /k:sonar-${username} /n:sonar-${username} /v:1.0"
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
                withSonarQubeEnv('Test_Sonar') {
                    bat "${scannerHome}\\SonarScanner.MSBuild.exe end"
                }
            }
        }

        stage('Create Docker Image') {
            steps {
                echo 'Docker image step'
                bat 'dotnet publish -c Release'
                bat "docker build -t i-${username}-master --no-cache -f Dockerfile ."
            }
        }

        stage('Push Docker Image') {
            steps {
                echo 'Push docker image to docker hub step'
                bat "docker tag i-${username}-master ${registry}:${BUILD_NUMBER}"

                withDockerRegistry([credentialsId: 'DockerHub', url: '']) {
                    bat "docker push ${registry}:${BUILD_NUMBER}"
                }
            }
        }

        stage('Docker Deployment') {
            steps {
                echo 'Docker deployment step'
                script {
                    if (env.container_id != null) {
                        echo 'Stop and remove existing container'
                        bat "docker stop c-${username}-master && docker rm c-${username}-master"
                    }
                    bat "docker run --name c-${username}-master -d -p ${docker_port}:80 ${registry}:${BUILD_NUMBER}"
                }
            }
        }
    }
}
