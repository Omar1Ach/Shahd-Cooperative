# ShahdCooperative - Project Structure Setup Script

Write-Host "Setting up ShahdCooperative folder structure..." -ForegroundColor Green

# Domain Layer Folders
$domainPath = "ShahdCooperative.Domain"
New-Item -ItemType Directory -Force -Path "$domainPath\Entities" | Out-Null
New-Item -ItemType Directory -Force -Path "$domainPath\Enums" | Out-Null
New-Item -ItemType Directory -Force -Path "$domainPath\Interfaces\Repositories" | Out-Null
New-Item -ItemType Directory -Force -Path "$domainPath\Interfaces\Services" | Out-Null
New-Item -ItemType Directory -Force -Path "$domainPath\Exceptions" | Out-Null

# Application Layer Folders
$appPath = "ShahdCooperative.Application"
New-Item -ItemType Directory -Force -Path "$appPath\DTOs\Products" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\DTOs\Orders" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\DTOs\Customers" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\DTOs\Feedback" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\DTOs\Reports" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Products\Commands\CreateProduct" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Products\Commands\UpdateProduct" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Products\Commands\DeleteProduct" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Products\Queries\GetProducts" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Products\Queries\GetProductById" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Orders\Commands\CreateOrder" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Orders\Commands\UpdateOrderStatus" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Orders\Commands\CancelOrder" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Orders\Queries\GetOrders" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Orders\Queries\GetOrderById" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Inventory\Commands\UpdateStock" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Inventory\Queries\GetLowStockProducts" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Customers\Queries\GetCustomers" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Feedback\Commands\CreateFeedback" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Feedback\Queries\GetFeedback" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Features\Reports\Queries\GetSalesReport" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Mappings" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Validators" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Common\Behaviors" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Common\Exceptions" | Out-Null
New-Item -ItemType Directory -Force -Path "$appPath\Common\Models" | Out-Null

# Infrastructure Layer Folders
$infraPath = "ShahdCooperative.Infrastructure"
New-Item -ItemType Directory -Force -Path "$infraPath\Persistence\Configurations" | Out-Null
New-Item -ItemType Directory -Force -Path "$infraPath\Persistence\Repositories" | Out-Null
New-Item -ItemType Directory -Force -Path "$infraPath\ExternalServices" | Out-Null
New-Item -ItemType Directory -Force -Path "$infraPath\MessageBroker\Events" | Out-Null
New-Item -ItemType Directory -Force -Path "$infraPath\Common" | Out-Null

# API Layer Folders
$apiPath = "ShahdCooperative.API"
New-Item -ItemType Directory -Force -Path "$apiPath\Controllers" | Out-Null
New-Item -ItemType Directory -Force -Path "$apiPath\Middleware" | Out-Null
New-Item -ItemType Directory -Force -Path "$apiPath\Filters" | Out-Null
New-Item -ItemType Directory -Force -Path "$apiPath\Extensions" | Out-Null

Write-Host "Folder structure created successfully!" -ForegroundColor Green
