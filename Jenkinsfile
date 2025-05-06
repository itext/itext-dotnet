#!/usr/bin/env groovy
@Library('pipeline-library')_

def repoName = "itextcore"
def dependencyRegex = ""
def solutionFile = "iTextCore.sln"
def frameworksToTest = "net461"
def frameworksToTestForMainBranches = "net461;netcoreapp2.0"

withEnv(
    ['ITEXT_VERAPDFVALIDATOR_ENABLE_SERVER=true', 
     'ITEXT_VERAPDFVALIDATOR_PORT=8091']) {
    automaticDotnetBuild(repoName, dependencyRegex, solutionFile, frameworksToTest, frameworksToTestForMainBranches)
}
