#!/usr/bin/env groovy
@Library('pipeline-library')_

def repoName = "itextcore"
def dependencyRegex = ""
def solutionFile = "iTextCore.sln"
def frameworksToTest = "net461"
def frameworksToTestForMainBranches = "net461;netcoreapp2.0"

automaticDotnetBuild(repoName, dependencyRegex, solutionFile, frameworksToTest, frameworksToTestForMainBranches)