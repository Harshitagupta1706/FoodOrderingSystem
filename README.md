# BiteRush Food Ordering System — .NET 10

An ASP.NET Core MVC food ordering project with SQLite + Entity Framework Core, session cart, search/category filtering, checkout, order persistence, admin menu/order management, and optional Razorpay Test Mode checkout.

## Requirements
- .NET 10 SDK
- Internet connection for restoring NuGet packages and Razorpay checkout assets
- Razorpay account only if you want to test online payment. Cash on Delivery works without Razorpay keys.

## Run locally
Open this folder in VS Code, open Terminal in the project directory, then run:

```powershell
dotnet --info
dotnet restore
dotnet run
```

Open the URL printed in Terminal (normally `http://localhost:5000`).

The SQLite database `foodordering.db` is created automatically and sample menu items are seeded on first run. Do not delete this file if you want to preserve your test orders.

## Admin
Open `/Admin/Login`. Demo access code: `FOOD2026`.

Change `Admin:AccessCode` in `appsettings.json` before any public deployment. This is a student demo admin login, not production-grade identity management.

## Online payment (Razorpay Test Mode)
The project includes a Razorpay Checkout flow. To enable it:

1. Create/sign in to a Razorpay account and enable **Test Mode**.
2. Get your **Test Key ID** and **Test Key Secret** from the Razorpay dashboard.
3. In `appsettings.json`, fill in the two fields under `Razorpay`:

```json
"Razorpay": {
  "KeyId": "rzp_test_your_key_id",
  "KeySecret": "your_test_key_secret"
}
```

4. Stop and restart the app, then choose **Online payment** at checkout.
5. Use Razorpay's documented test payment details from its official documentation/dashboard.

Do not commit real keys to source control or share them in chat. Never use live keys for a college demo. This implementation creates a Razorpay order on the server and verifies the returned HMAC signature server-side before marking the order Paid. If a payment is dismissed or fails, it is not marked Paid. The order remains pending payment until the flow is completed.

## Main features
- Responsive food menu with category filters and search
- Session cart: add, update quantity, remove, clear
- Server-side price recalculation at checkout
- Data-annotated customer validation
- Cash on Delivery orders
- Optional Razorpay Test Mode payment with server-side signature verification
- SQLite persistence for menu items, orders and order items
- Admin login, order dashboard/status updates, add menu items, show/hide items
- Anti-forgery tokens on POST forms

## Important notes
- This is an educational project, not a production-ready commercial payment system.
- Use only Razorpay Test Mode for demonstrations until you have reviewed production security, HTTPS deployment, webhook handling and reconciliation.
- The app uses `EnsureCreated()` for simple first-run database creation. If you change database schema after a database already exists, back up/delete the local `foodordering.db` only if you are comfortable losing its test data, then restart to recreate it.
- External food images and Google Fonts require internet access.
- This package has not been compiled or run in this file-generation environment. After download, run `dotnet restore` and `dotnet build`; send any error output for correction.
