pipeline{
    agent { label 'aaa-jenkins2' }
    environment {
        BAMS_CREDS = credentials('a5e809ef-6598-4f88-a765-88b4b3387cc5')
		gitCredsId = "a5e809ef-6598-4f88-a765-88b4b3387cc5"        
		gitPushURL = "git.sami.int.thomsonreuters.com/aaa/aaa-api-template"
		gitRepoURL = "https://${gitPushURL}"
        GIT_CREDS = credentials('CHRIS_BAMS')
    }
    stages{
        stage("Start"){
            steps{
                updateGitlabCommitStatus name: 'build', state: 'running'
            }
		}
        stage("Restore packages"){          
            steps{
                bat "nuget sources update -Name \"BAMS (AWS)\" -username ${BAMS_CREDS_USR} -password ${BAMS_CREDS_PSW}"
                bat "dotnet restore"
            }
        }
		stage('Build + SonarQube analysis') {
			steps{
				withSonarQubeEnv('AAA_Sonar') {
					bat "C:\\sonar-scanner-msbuild\\SonarQube.Scanner.MSBuild.exe begin /k:aaa-api-template /d:sonar.cs.nunit.reportsPaths=\"**/TestResults/TestResults.xml\" /d:sonar.cs.opencover.reportsPaths=\"OpenCover.xml\""
					bat "dotnet build --configuration Release"
					bat "opencover\\4.7.922\\tools\\OpenCover.Console.exe -target:\"C:\\Program Files\\dotnet\\dotnet.exe\" -targetargs:\"test --configuration Release --logger nunit --filter TestCategory!=Integration\" -register -oldStyle -filter:\"+[*]* -[*]*Startup\" -output:opencover.xml -excludebyattribute:*.ExcludeFromCodeCoverageAttribute"
					bat "C:\\sonar-scanner-msbuild\\SonarQube.Scanner.MSBuild.exe end"
				}
			}
			post{
                always{
                    nunit testResultsPattern: '**/TestResults.xml', failIfNoResults: false
                }
            }
		}
		stage("Quality Gate") {
            steps {
				sleep 15
                timeout(time: 10, unit: 'MINUTES') {
                    waitForQualityGate abortPipeline: true
                }
            }
        }
        stage("Publish Branch Build to BAMS"){
			when{
				not{
					branch 'master'
				}
			}
            steps{
				script{
					def latestVersion = sh returnStdout: true, script: 'git for-each-ref refs/tags/snapshot refs/tags/develop --sort=-taggerdate --format=\'%(refname:short)\' --count=1 || echo snapshot/0.0.0.0'
					def version = latestVersion.split('/')[1]
					println latestVersion
					println version
					def (major, minor, patch, build) = version.tokenize('.').collect { it.toInteger() }
					build++
					env.VERSIONNUMBER = major + "." + minor + "." + patch + "." +  build
					env.ZIPFILE = "Refinitiv.Aaa.GuissApi-" + env.VERSIONNUMBER + ".zip"
                    env.DEPLOYMENTFOLDER = "template-api-deployment-" + env.VERSIONNUMBER
				}
                bat "mkdir ${env.DEPLOYMENTFOLDER}"
				bat "dotnet lambda package /p:Version=${env.VERSIONNUMBER} --configuration Release -o ${env.ZIPFILE} -pl Refinitiv.Aaa.GuissApi"
                bat "cp ./${env.ZIPFILE} ./${env.DEPLOYMENTFOLDER}"
                zip zipFile: "${env.DEPLOYMENTFOLDER}/Refinitiv.Aaa.GuissApi-${env.VERSIONNUMBER}-terraform.zip", dir: "terraform", archive: false
                zip zipFile: "${env.DEPLOYMENTFOLDER}/Refinitiv.Aaa.GuissApi.PostmanTests-${env.VERSIONNUMBER}.zip", dir: "Refinitiv.Aaa.GuissApi.PostmanTests", archive: false
				zip zipFile: "${env.DEPLOYMENTFOLDER}/Refinitiv.Aaa.GuissApi.RDPArtefacts-${env.VERSIONNUMBER}.zip", dir: "RDP Artefacts", archive: false
                rtServer id: 'BAMS', url: 'https://bams-aws.refinitiv.com/artifactory', username: BAMS_CREDS_USR, password: BAMS_CREDS_PSW
                rtUpload(
                    serverId: 'BAMS',
                    spec:
                        """{
                                "files": [
                                    {
                                        "pattern": "./${env.DEPLOYMENTFOLDER}/",
                                        "target": "default.generic.cloud/aaa/aws/templateapi/snapshots/${gitlabSourceBranch}/",
                                        "flat": false
                                    }
                                ]

                        }"""
                )
                rtPublishBuildInfo serverId: 'BAMS'
				sh """
					git tag -a snapshot/${env.VERSIONNUMBER} -m  '${env.VERSIONNUMBER} - Release to BAMS successful'
					git push https://${GIT_CREDS_USR}:${GIT_CREDS_PSW}@${env.gitPushURL} --tags
				"""
            }
        }
    }
    post{
        success{
            updateGitlabCommitStatus name: 'build', state: 'success'
            addGitLabMRComment comment: ':white_check_mark: Jenkins build SUCCESS'
        }
        failure{
            updateGitlabCommitStatus name: 'build', state: 'failed'
            addGitLabMRComment comment: ':x: Jenkins build FAILURE'
        }
		cleanup{
			cleanWs()
		}
    }
}