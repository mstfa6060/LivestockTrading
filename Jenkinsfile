pipeline {
    agent any

    stages {
        stage('Setup') {
            steps {
                script {
                    echo "��� Setup işlemleri başlatılıyor..."
                    sh 'dotnet restore'
                }
            }
        }

        stage('Build ErrorCodeExporter') {
            steps {
                sh 'dotnet build Jobs/SpecialPurpose/DevTasks/ErrorCodeExporter/ErrorCodeExporter.csproj --configuration Release'
                sh 'dotnet run --project Jobs/SpecialPurpose/DevTasks/ErrorCodeExporter/ErrorCodeExporter.csproj'
            }
        }

        stage('Generate ArfBlocks Docs') {
            steps {
                sh 'arfblocks-cli exec --file _devops/arfblocks-cli/hirovo.arfblocks-cli.json'
                sh 'arfblocks-cli exec --file _devops/arfblocks-cli/hirovo-iam.arfblocks-cli.json'
            }
        }

        stage('Publish Artifacts') {
            steps {
                echo "��� Artifaktlar hazırlanıyor (opsiyonel)..."
            }
        }
    }
}
