pipeline {
    agent any

    environment {
        DOCKER_USERNAME = 'mstfaock'
        DOCKER_REGISTRY = 'docker.io'
        TAG = 'dev-latest'
        DOCKER_CREDENTIALS = credentials('docker-hub-credentials')
        DEV_SERVER_HOST = credentials('dev-server-host')
        DEV_SERVER_USER = credentials('dev-server-user')
    }

    options {
        buildDiscarder(logRotator(numToKeepStr: '10'))
        disableConcurrentBuilds()
        timeout(time: 30, unit: 'MINUTES')
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Docker Login') {
            steps {
                sh '''
                    echo $DOCKER_CREDENTIALS_PSW | docker login -u $DOCKER_CREDENTIALS_USR --password-stdin
                '''
            }
        }

        stage('Build & Push Images') {
            parallel {
                stage('LivestockTrading API') {
                    steps {
                        script {
                            buildAndPush('livestocktrading-api', '_devops/docker/Dockerfile.livestocktrading-api')
                        }
                    }
                }
                stage('IAM API') {
                    steps {
                        script {
                            buildAndPush('livestocktrading-iam-api', '_devops/docker/Dockerfile.iam-api')
                        }
                    }
                }
                stage('FileProvider API') {
                    steps {
                        script {
                            buildAndPush('livestocktrading-fileprovider-api', '_devops/docker/Dockerfile.fileprovider-api')
                        }
                    }
                }
                stage('IAM Mail Worker') {
                    steps {
                        script {
                            buildAndPush('livestocktrading-iam-mail-worker', '_devops/docker/Dockerfile.iam-mail-worker')
                        }
                    }
                }
                stage('IAM SMS Worker') {
                    steps {
                        script {
                            buildAndPush('livestocktrading-iam-sms-worker', '_devops/docker/Dockerfile.iam-sms-worker')
                        }
                    }
                }
                stage('Resource Seeder') {
                    steps {
                        script {
                            buildAndPush('livestocktrading-resource-seeder', '_devops/docker/Dockerfile.resource-seeder')
                        }
                    }
                }
            }
        }

        stage('Deploy to DEV') {
            steps {
                sshagent(credentials: ['dev-server-ssh-key']) {
                    sh '''
                        ssh -o StrictHostKeyChecking=no ${DEV_SERVER_USER}@${DEV_SERVER_HOST} << 'ENDSSH'
                            cd /opt/livestocktrading

                            echo "📥 Git repository güncelleniyor..."
                            git fetch origin dev
                            git reset --hard origin/dev
                            git submodule update --init --recursive

                            echo "🐳 Docker image'ları pull ediliyor..."
                            docker compose -p livestock_dev \
                              -f docker-compose.yml \
                              -f docker-compose.dev.yml \
                              --env-file .env.dev pull

                            echo "🌱 ResourceSeeder çalıştırılıyor..."
                            docker run --rm \
                              --network livestock_dev_livestocktrading-network \
                              --env-file .env.dev \
                              mstfaock/livestocktrading-resource-seeder:dev-latest \
                              development || echo "⚠️ ResourceSeeder failed but continuing..."

                            echo "🚀 Servisler başlatılıyor..."
                            docker compose -p livestock_dev \
                              -f docker-compose.yml \
                              -f docker-compose.dev.yml \
                              --env-file .env.dev up -d

                            echo "🧹 Temizlik yapılıyor..."
                            docker image prune -f

                            echo "✅ DEV deployment tamamlandı!"

                            echo "📊 Container durumu:"
                            docker compose -p livestock_dev ps
ENDSSH
                    '''
                }
            }
        }
    }

    post {
        always {
            sh 'docker logout || true'
        }
        success {
            echo '✅ Pipeline başarıyla tamamlandı!'
        }
        failure {
            echo '❌ Pipeline başarısız oldu!'
        }
    }
}

def buildAndPush(String imageName, String dockerfile) {
    def fullImageName = "${DOCKER_USERNAME}/${imageName}"
    def commitTag = "dev-${GIT_COMMIT.take(7)}"

    sh """
        docker build -t ${fullImageName}:${TAG} -t ${fullImageName}:${commitTag} -f ${dockerfile} .
        docker push ${fullImageName}:${TAG}
        docker push ${fullImageName}:${commitTag}
    """
}
