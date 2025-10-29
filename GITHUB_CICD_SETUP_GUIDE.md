# GitHub CI/CD Setup Guide - Step by Step

## Overview
This guide shows how to set up Continuous Integration and Continuous Deployment (CI/CD) using GitHub Actions for your project.

---

## Part 1: Understanding the CI/CD Workflow

### What the Workflow Does

**On Every Push/Pull Request:**
1. ✅ Runs all unit tests
2. ✅ Builds the project
3. ✅ Reports test results
4. 🚀 Builds Docker image (main branch only)
5. 🚀 Pushes to GitHub Container Registry (main branch only)
6. 🚀 Deploys to production (optional, main branch only)

---

## Part 2: GitHub Repository Setup

### Step 1: Push Your Code to GitHub

#### 1.1: Create GitHub Repository
1. Go to https://github.com
2. Click the **"+"** icon in top-right corner
3. Click **"New repository"**
4. Fill in details:
   - **Repository name:** `ShahdCooperative` (or your project name)
   - **Description:** "ShahdCooperative Authentication Service"
   - **Visibility:** Choose Public or Private
   - **Do NOT** initialize with README (you already have code)
5. Click **"Create repository"**

#### 1.2: Push Existing Code
```bash
cd C:\Users\Predator\ShahdCooperative

# Add remote (replace with your repository URL)
git remote add origin https://github.com/YOUR_USERNAME/ShahdCooperative.git

# Verify remote was added
git remote -v

# Push code
git push -u origin main
```

**Expected Output:**
```
Enumerating objects: 157, done.
Counting objects: 100% (157/157), done.
Delta compression using up to 8 threads
Compressing objects: 100% (98/98), done.
Writing objects: 100% (157/157), 45.23 KiB | 2.26 MiB/s, done.
Total 157 (delta 43), reused 0 (delta 0), pack-reused 0
remote: Resolving deltas: 100% (43/43), done.
To https://github.com/YOUR_USERNAME/ShahdCooperative.git
 * [new branch]      main -> main
branch 'main' set up to track 'origin/main'.
```

### Step 2: Verify CI/CD Workflow File

The workflow file should already exist at: `.github/workflows/ci-cd.yml`

If not, it was created in previous steps. Verify:
```bash
cat .github/workflows/ci-cd.yml
```

---

## Part 3: GitHub Actions Configuration

### Step 1: Access GitHub Actions

1. Go to your repository on GitHub
2. Click the **"Actions"** tab at the top
3. You should see the "CI/CD Pipeline" workflow

**Screenshot location:**
```
https://github.com/YOUR_USERNAME/ShahdCooperative/actions
```

### Step 2: Enable GitHub Actions (if disabled)

If you see a message saying "Workflows aren't being run":
1. Click the green **"I understand my workflows, go ahead and enable them"** button
2. Click **"Enable workflow"**

### Step 3: Trigger First Workflow Run

#### Method 1: Push a Commit
```bash
# Make a small change
echo "" >> README.md

# Commit and push
git add README.md
git commit -m "Trigger CI/CD workflow"
git push
```

#### Method 2: Manually Trigger (if configured)
1. Go to **Actions** tab
2. Click on **"CI/CD Pipeline"** in left sidebar
3. Click **"Run workflow"** button
4. Select branch: **main**
5. Click green **"Run workflow"** button

### Step 4: Monitor Workflow Execution

1. Go to **Actions** tab
2. Click on the running workflow (yellow dot 🟡)
3. Watch the progress in real-time

**Workflow Stages:**
```
1. test (Run Tests)
   ├── Setup .NET
   ├── Restore dependencies
   ├── Build
   └── Run tests

2. build-and-push (Build and Push Docker Image)
   ├── Log in to Container Registry
   ├── Extract metadata
   └── Build and push Docker image

3. deploy (Deploy to Production)
   └── Deploy notification
```

**Status Indicators:**
- 🟡 Yellow dot = Running
- ✅ Green checkmark = Success
- ❌ Red X = Failed
- ⏭️ Skipped = Not run (conditions not met)

### Step 5: View Logs

1. Click on a workflow run
2. Click on a job (e.g., "Run Tests")
3. Click on a step to expand logs
4. View detailed output

---

## Part 4: Container Registry Setup

### Step 1: Enable GitHub Container Registry

GitHub Container Registry (GHCR) is automatically enabled. The workflow uses it to store Docker images.

**Container Registry URL:**
```
ghcr.io/YOUR_USERNAME/shahdcooperative:latest
```

### Step 2: View Published Images

1. Go to your GitHub profile: https://github.com/YOUR_USERNAME
2. Click **"Packages"** tab
3. You'll see your Docker images listed

**Or visit directly:**
```
https://github.com/YOUR_USERNAME?tab=packages
```

### Step 3: Make Package Public (Optional)

By default, packages are private. To make public:

1. Click on your package
2. Click **"Package settings"** (bottom right)
3. Scroll to **"Danger Zone"**
4. Click **"Change visibility"**
5. Select **"Public"**
6. Type package name to confirm
7. Click **"I understand the consequences, change package visibility"**

---

## Part 5: Secrets Management

### Step 1: Access Repository Secrets

1. Go to your repository on GitHub
2. Click **"Settings"** tab
3. In left sidebar, click **"Secrets and variables"** → **"Actions"**

### Step 2: Add Required Secrets

The workflow already uses `GITHUB_TOKEN` (automatically provided).

**For additional secrets (if needed):**

#### Add Docker Hub Credentials (Optional)
1. Click **"New repository secret"**
2. Name: `DOCKERHUB_USERNAME`
3. Secret: Your Docker Hub username
4. Click **"Add secret"**

Repeat for:
- `DOCKERHUB_TOKEN` - Your Docker Hub access token

#### Add Deployment Secrets (Optional)
1. Name: `SSH_PRIVATE_KEY`
2. Secret: Your SSH private key for deployment server
3. Click **"Add secret"**

Repeat for:
- `DEPLOY_HOST` - Your server hostname
- `DEPLOY_USER` - SSH username
- `DEPLOY_PATH` - Deployment path on server

### Step 3: Use Secrets in Workflow

Secrets are accessed using: `${{ secrets.SECRET_NAME }}`

Example:
```yaml
- name: Deploy to server
  env:
    SSH_KEY: ${{ secrets.SSH_PRIVATE_KEY }}
    HOST: ${{ secrets.DEPLOY_HOST }}
  run: |
    echo "$SSH_KEY" > key.pem
    chmod 600 key.pem
    ssh -i key.pem $HOST "docker pull ghcr.io/user/app:latest"
```

---

## Part 6: Branch Protection Rules

### Step 1: Protect Main Branch

1. Go to **Settings** → **Branches**
2. Click **"Add branch protection rule"**
3. Branch name pattern: `main`
4. Enable these options:
   - ✅ **Require a pull request before merging**
   - ✅ **Require status checks to pass before merging**
     - Search and select: **"test"**
   - ✅ **Require branches to be up to date before merging**
   - ✅ **Include administrators** (optional)
5. Click **"Create"**

**What this does:**
- Prevents direct pushes to main branch
- Requires pull requests
- Requires tests to pass before merging

### Step 2: Create Feature Branch Workflow

```bash
# Create feature branch
git checkout -b feature/new-feature

# Make changes
# ... edit files ...

# Commit changes
git add .
git commit -m "Add new feature"

# Push to GitHub
git push -u origin feature/new-feature
```

### Step 3: Create Pull Request

1. Go to your repository on GitHub
2. You'll see a banner: **"Compare & pull request"**
3. Click the button
4. Fill in PR details:
   - Title: Brief description
   - Description: Detailed changes
5. Click **"Create pull request"**
6. Wait for CI/CD checks to complete
7. If all checks pass ✅, click **"Merge pull request"**

---

## Part 7: Notifications Setup

### Step 1: Email Notifications

**Automatic (default):**
- GitHub sends emails for workflow failures
- Configure at: https://github.com/settings/notifications

### Step 2: Slack Notifications (Optional)

Add to workflow:
```yaml
- name: Slack Notification
  if: always()
  uses: slackapi/slack-github-action@v1
  with:
    webhook-url: ${{ secrets.SLACK_WEBHOOK_URL }}
    payload: |
      {
        "text": "Build ${{ job.status }}: ${{ github.repository }}"
      }
```

### Step 3: Discord Notifications (Optional)

Add to workflow:
```yaml
- name: Discord Notification
  if: always()
  uses: sarisia/actions-status-discord@v1
  with:
    webhook: ${{ secrets.DISCORD_WEBHOOK }}
```

---

## Part 8: Monitoring and Debugging

### View Workflow Runs

**Recent runs:**
```
https://github.com/YOUR_USERNAME/ShahdCooperative/actions
```

**Specific workflow:**
```
https://github.com/YOUR_USERNAME/ShahdCooperative/actions/workflows/ci-cd.yml
```

### Download Artifacts

If workflow creates artifacts:
1. Go to workflow run
2. Scroll to **"Artifacts"** section
3. Click artifact name to download

### Re-run Failed Jobs

1. Go to failed workflow run
2. Click **"Re-run jobs"** (top right)
3. Select **"Re-run failed jobs"** or **"Re-run all jobs"**

### Cancel Running Workflow

1. Go to running workflow
2. Click **"Cancel workflow"** (top right)

---

## Part 9: Advanced Configuration

### Workflow Triggers

**Current triggers:**
```yaml
on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]
```

**Add manual trigger:**
```yaml
on:
  workflow_dispatch:  # Adds "Run workflow" button
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]
```

**Add schedule:**
```yaml
on:
  schedule:
    - cron: '0 2 * * *'  # Daily at 2 AM UTC
  push:
    branches: [ main, develop ]
```

### Environment Variables

**Repository-level:**
1. Go to **Settings** → **Secrets and variables** → **Actions**
2. Click **"Variables"** tab
3. Click **"New repository variable"**
4. Add variables (not secrets, visible in logs)

**Workflow-level:**
```yaml
env:
  DOTNET_VERSION: '8.0.x'
  BUILD_CONFIGURATION: 'Release'
```

**Job-level:**
```yaml
jobs:
  test:
    env:
      TEST_DATABASE: 'TestDB'
```

### Matrix Builds

Test multiple versions:
```yaml
jobs:
  test:
    strategy:
      matrix:
        dotnet-version: ['6.0.x', '7.0.x', '8.0.x']
        os: [ubuntu-latest, windows-latest, macos-latest]
    runs-on: ${{ matrix.os }}
    steps:
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ matrix.dotnet-version }}
```

---

## Part 10: Common Issues and Solutions

### Issue 1: Workflow Not Running

**Causes:**
- Workflow file in wrong location
- YAML syntax error
- Actions disabled

**Solution:**
1. Verify file location: `.github/workflows/ci-cd.yml`
2. Validate YAML: https://www.yamllint.com/
3. Check Actions tab for errors

### Issue 2: Tests Failing

**Check:**
```bash
# Run tests locally
dotnet test

# View detailed output
dotnet test --logger "console;verbosity=detailed"
```

**Common fixes:**
- Update test project references
- Fix connection strings in tests
- Ensure test databases are accessible

### Issue 3: Docker Build Failing

**Check Dockerfile:**
```bash
# Test locally
docker build -t test-image .

# View build logs
docker build --progress=plain -t test-image .
```

**Common fixes:**
- Verify .NET SDK version matches
- Check file paths in COPY commands
- Ensure all project references are correct

### Issue 4: Permission Denied (Docker Push)

**Error:** `denied: permission_denied`

**Solution:**
1. Ensure package exists and you have write access
2. Check token permissions
3. Make package public or grant access

### Issue 5: Secrets Not Available

**Error:** `secret not found`

**Solution:**
1. Verify secret name matches exactly (case-sensitive)
2. Check secret is in correct location (repo/org/environment)
3. Ensure workflow has permission to access secrets

---

## Part 11: Deployment Configuration

### Option 1: Deploy to Azure

```yaml
- name: Deploy to Azure Web App
  uses: azure/webapps-deploy@v2
  with:
    app-name: 'your-app-name'
    publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
    images: 'ghcr.io/${{ github.repository }}:${{ github.sha }}'
```

### Option 2: Deploy to AWS

```yaml
- name: Deploy to AWS ECS
  uses: aws-actions/amazon-ecs-deploy-task-definition@v1
  with:
    task-definition: task-definition.json
    service: your-service
    cluster: your-cluster
```

### Option 3: Deploy to Your Server (SSH)

```yaml
- name: Deploy to Server
  env:
    SSH_KEY: ${{ secrets.SSH_PRIVATE_KEY }}
  run: |
    echo "$SSH_KEY" > key.pem
    chmod 600 key.pem
    ssh -o StrictHostKeyChecking=no -i key.pem user@server << 'EOF'
      cd /app
      docker-compose pull
      docker-compose up -d
    EOF
```

### Option 4: Deploy to Kubernetes

```yaml
- name: Deploy to Kubernetes
  uses: azure/k8s-deploy@v1
  with:
    manifests: |
      k8s/deployment.yml
      k8s/service.yml
    images: |
      ghcr.io/${{ github.repository }}:${{ github.sha }}
```

---

## Part 12: Monitoring Success

### Check Workflow Status

**Badge in README.md:**
```markdown
![CI/CD](https://github.com/YOUR_USERNAME/ShahdCooperative/actions/workflows/ci-cd.yml/badge.svg)
```

### View Deployment History

1. Go to **Actions** tab
2. Filter by workflow
3. View all runs and their status

### Test Results

Test results are automatically published using `dorny/test-reporter`.

**View:**
1. Go to workflow run
2. Click **"Summary"**
3. See **"Test Results"** section

---

## Quick Reference Commands

```bash
# Check workflow locally
act -l  # Requires: https://github.com/nektos/act

# Validate workflow YAML
yamllint .github/workflows/ci-cd.yml

# View GitHub CLI workflows
gh workflow list
gh workflow view ci-cd.yml
gh run list
gh run watch

# Trigger workflow manually
gh workflow run ci-cd.yml

# View logs
gh run view --log

# Download artifacts
gh run download <run-id>
```

---

## Next Steps

1. ✅ Workflow running successfully
2. ✅ Tests passing
3. ✅ Docker images building
4. ⏭️ Configure deployment target
5. ⏭️ Set up monitoring/alerts
6. ⏭️ Add performance tests
7. ⏭️ Implement staging environment

---

## Support and Resources

- **GitHub Actions Docs:** https://docs.github.com/actions
- **GitHub Container Registry:** https://docs.github.com/packages
- **Workflow Syntax:** https://docs.github.com/actions/reference/workflow-syntax-for-github-actions
- **Marketplace:** https://github.com/marketplace?type=actions
