#!/usr/bin/env groovy
@Library('pipeline-library')_

def vars = setBranchDependentVars(env.BRANCH_NAME)

pipeline {

    agent { label 'windows' }

    options {
        ansiColor('xterm')
        buildDiscarder(
            logRotator(
                numToKeepStr: vars.buildNumToKeep,
                artifactNumToKeepStr: vars.buildArtifactNumToKeep,
                daysToKeepStr: vars.buildDaysToKeep,
                artifactDaysToKeepStr: vars.buildArtifactDaysToKeep
            )
        )
        parallelsAlwaysFailFast()
        retry(1)
        skipStagesAfterUnstable()
        timeout(time: 60, unit: 'MINUTES')
        timestamps()
    }

    triggers {
        cron(vars.schedule)
    }

    stages {
        stage('Clean workspace') {
            options {
                timeout(time: 5, unit: 'MINUTES')
            }
            steps {
                cleanWs deleteDirs: true, patterns: [
                    [pattern: 'packages', type: 'INCLUDE'],
                    [pattern: 'global-packages', type: 'INCLUDE'],
                    [pattern: 'tmp/NuGetScratch', type: 'INCLUDE'],
                    [pattern: 'http-cache', type: 'INCLUDE'],
                    [pattern: 'plugins-cache', type: 'INCLUDE'],
                    [pattern: '**/obj', type: 'INCLUDE'],
                    [pattern: '**/bin', type: 'INCLUDE']
                ]
            }
        }
        stage('Compile') {
            options {
                timeout(time: 20, unit: 'MINUTES')
            }
            steps {
                withEnv(["NUGET_PACKAGES=${env.WORKSPACE}/global-packages", "temp=${env.WORKSPACE}/tmp/NuGetScratch", "NUGET_HTTP_CACHE_PATH=${env.WORKSPACE}/http-cache", "NUGET_PLUGINS_CACHE_PATH=${env.WORKSPACE}/plugins-cache", "gsExec=${gsExec}", "compareExec=${compareExec}"]) {
                    bat "\"${env.NuGet}\" restore iTextCore.netstandard.sln"
                    bat "dotnet restore iTextCore.netstandard.sln"
                    bat "dotnet build iTextCore.netstandard.sln --configuration Release --source ${env.WORKSPACE}/packages"
                    script {
                         createPackAllFile(findFiles(glob: '**/*.nuspec'))
                         load 'packAll.groovy'
                    }
                }
            }
        }
        stage('Run Tests') {
            options {
                timeout(time: 60, unit: 'MINUTES')
            }
            steps {
                withEnv(["NUGET_PACKAGES=${env.WORKSPACE}/global-packages", "temp=${env.WORKSPACE}/tmp/NuGetScratch", "NUGET_HTTP_CACHE_PATH=${env.WORKSPACE}/http-cache", "NUGET_PLUGINS_CACHE_PATH=${env.WORKSPACE}/plugins-cache", "gsExec=${gsExec}", "compareExec=${compareExec}"]) {
                    script {
                        createRunTestDllsFile(findFiles(glob: '**/itext.*.tests.dll'))
                        load 'runTestDlls.groovy'
                        createRunTestCsProjsFile(findFiles(glob: '**/itext.*.tests.netstandard.csproj'))
                        load 'runTestCsProjs.groovy'
                    }
                }
            }
        }
        stage('Artifactory Deploy') {
            options {
                timeout(time: 5, unit: 'MINUTES')
            }
            when {
                anyOf {
                    branch "master"
                    branch "develop"
                }
            }
            steps {
                script {
                    getAndConfigureJFrogCLI()
                    findFiles(glob: '*.nupkg').each { item ->
                        upload(item)
                    }
                }
            }
        }
        stage('Branch Artifactory Deploy') {
            options {
                timeout(time: 5, unit: 'MINUTES')
            }
            when {
                not {
                    anyOf {
                        branch "master"
                        branch "develop"
                    }
                }
            }
            steps {
                script {
                    if (env.GIT_URL) {
                        getAndConfigureJFrogCLI()
                        repoName = ("${env.GIT_URL}" =~ /(.*\/)(.*)(\.git)/)[ 0 ][ 2 ]
                        findFiles(glob: '*.nupkg').each { item ->
                            sh "./jfrog rt u \"${item.path}\" branch-artifacts/${env.BRANCH_NAME}/${repoName}/dotnet/ --recursive=false --build-name ${env.BRANCH_NAME} --build-number ${env.BUILD_NUMBER} --props \"vcs.revision=${env.GIT_COMMIT};repo.name=${repoName}\""
                        }
                    }
                }
            }
        }
        stage('Archive Artifacts') {
            options {
                timeout(time: 5, unit: 'MINUTES')
            }
            steps {
                archiveArtifacts allowEmptyArchive: true, artifacts: '*.nupkg'
            }
        }
    }

    post {
        always {
            echo 'One way or another, I have finished \uD83E\uDD16'
        }
        success {
            echo 'I succeeeded! \u263A'
            cleanWs deleteDirs: true
        }
        unstable {
            echo 'I am unstable \uD83D\uDE2E'
        }
        failure {
            echo 'I failed \uD83D\uDCA9'
        }
        changed {
            echo 'Things were different before... \uD83E\uDD14'
        }
        fixed {
            script {
                if (vars.notifySlack) {
                    slackNotifier("#ci", currentBuild.currentResult, "${env.BRANCH_NAME} - Back to normal")
                }
            }
        }
        regression {
            script {
                if (vars.notifySlack) {
                    slackNotifier("#ci", currentBuild.currentResult, "${env.BRANCH_NAME} - First failure")
                }
            }
        }
    }

}

@NonCPS // has to be NonCPS or the build breaks on the call to .each
def createPackAllFile(list) {
    // creates file because the bat command brakes the loop
    def cmd = ''
    list.each { item ->
        if (!item.path.contains("packages")) {
            cmd = cmd + "bat '\"${env.NuGet.replace('\\','\\\\')}\" pack \"${item.path.replace('\\','\\\\')}\"'\n"
        }
    }
    writeFile file: 'packAll.groovy', text: cmd
}

@NonCPS // has to be NonCPS or the build breaks on the call to .each
def createRunTestDllsFile(list) {
    // creates file because the bat command brakes the loop
    def ws = "${env.WORKSPACE.replace('\\','\\\\')}"
    def nunit = "${env.'Nunit3-console'.replace('\\','\\\\')}"
    def cmd = ''
    list.each { item ->
        if (!item.path.contains("netcoreapp1.0") && !item.path.contains("obj")) {
            cmd = cmd + "bat '\"${nunit}\" \"${ws}\\\\${item.path.replace('\\','\\\\')}\" --result=${item.name}-TestResult.xml'\n"
        }
    }
    writeFile file: 'runTestDlls.groovy', text: cmd
}

@NonCPS // has to be NonCPS or the build breaks on the call to .each
def createRunTestCsProjsFile(list) {
    // creates file because the bat command brakes the loop
    def ws = "${env.WORKSPACE.replace('\\','\\\\')}"
    def cmd = ''
    list.each { item ->
        cmd = cmd + "bat 'dotnet test ${ws}\\\\${item.path.replace('\\','\\\\')} --framework netcoreapp1.0 --configuration Release --no-build --logger \"trx;LogFileName=results.trx\"'\n"
    }
    writeFile file: 'runTestCsProjs.groovy', text: cmd
}

@NonCPS
def upload(item) {
    def itemArray = (item =~ /(.*?)(\.[0-9]*\.[0-9]*\.[0-9]*(-SNAPSHOT)?\.nupkg)/)
    def dir = itemArray[ 0 ][ 1 ]
    sh "./jfrog rt u \"${item.path}\" nuget/${dir}/ --flat=false --build-name="${env.BRANCH_NAME}" --build-number=${env.BUILD_NUMBER}"
}


