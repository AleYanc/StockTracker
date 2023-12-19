# StockTracker
StockTracker is an API for stock management.
At the moment, the API functionality is focused on managing the products and sales of a business.
### All the endpoints are documented in PostMan [HERE](https://documenter.getpostman.com/view/11287464/2s9Ykhi4rg)
### Testing suite is [HERE](https://github.com/AleYanc/StockTracker.Tests)
The main entity is Product, having the following properties:
* _Id_ -> The product identifier. Every entity has an identifier.
* _Name_
* _Description_
* _ImagesUrl_ -> A JSON serialized object that stores all the urls corresponding to the product.
* _WidthCM_ -> Product width in centimeters
* _HeightCM_ -> Product height in centimeters
* _DepthCM_ -> Product depth in centimeters
* _WeightGs_ -> Product weight in grams
* _Cost_ -> Product cost. Different from the actual listing price.
* _Active_ -> boolean to check if the product is active or not.

## Secondary entities
### ProductCode
Useful to create unique codes for products. The default schema is a ten number code parsed into a string. Example '0000000001'.
Automatically creates when a new product is posted. It has a one-to-one relationship with the **Product** entity.
* _Code_
* _ProductId_ -> needed to build relationship with corresponding product. 
* _CreatedDate_
### PriceList
Different from the cost, a price list can contain multiple prices for a single product. Example, "DEBIT CARD" or "CASH". It has a one-to-many relationship with the **Product** entity.
* -PriceType_
* _Price_
* _ProductId_ -> needed to build relationship with corresponding product.
### Stock
Entity to keep track of a product stock. It has a one-to-one relationship with the **Product** entity.
* _ProductId_
* _Quantity_
### Sale
Useful for keeping track of business sales. It is an independent entity.
* _SaleDate_ -> date in which the sale was made. Needed to filter sales by x period of time (ex. day, week, month, etc)
*  _Status_ -> it will be based on the business owner. For example, a sale can have three status: "PENDING", "VERIFIED", "DECLINED".
*  _Total_ -> the total price of the sale. It gets automatically calculated in the controller, so the user does not have to.
*  _Currency_ -> the currency type. A currency exchange rate is planned for a future version.
*  _Products_ -> a JSON serialized object that contains the ids of the sale's products.
*  _PaymentMethod_
### Category
Useful to divide products into categories. It has a one-to-one relatiopnship with the **Product** entity.
* _Name_
### Supplier
Not yet implemented. Useful to search for products or update stock based on the supplier. It has a one-to-many relationship with the **Product** entity.
* _Active_
* _Name_ -> real first name of the supplier.
* _LastName_ -> real last name of the supplier.
* _CompanyName_ -> company name of the supplier.
* _BusinessNumber_ -> number or legal document used by the company as identification. For example, in Argentina it can be the CUIT.
* _ContactPhone_
* _ContactEmail_
* _Address_
* _Products_
### ImportHistory
This entity was made to keep track of the excel imports to the application. It is an independent entity.
* _ImportType_ -> import type depends on the imported entity. Can be Product or PriceList as of now.
* _FileName_ -> file name of the imported excel file.
* _FilePath_ -> path to where the file is uploaded.
* _ImportDate_
## Main functionalities
* CRUD for Products, Stock, PriceList, Categories and Sales.
* Mass-import of Products or PriceLists via excel file (**with a default excel template**).
