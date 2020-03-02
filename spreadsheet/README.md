# Spreadsheet Implementation

Having the content from your CMS display on a spreadsheet is super awesome. You can take inspiration from this [Medium article on how to consume a JSON API using Google sheets](https://medium.com/unly-org/how-to-consume-any-json-api-using-google-sheets-and-keep-it-up-to-date-automagically-fb6e94521abd).

Once you have the initial script set up according to the instructions in the article, use the following formula in cell A1 to display the data: 
```
=ImportJSON("https://cdn.umbraco.io/content/type?contentType=product&umb-project-alias=your-project-alias", "/_embedded/content/_updateDate,/_embedded/content/title,/_embedded/content/description,/_embedded/content/price,/_embedded/content/quantity,/_embedded/content/isOnSpecialToday,/_embedded/content/image/_url", "noInherit,noTruncate,allHeaders")
```
The [Lamington Inventory](https://docs.google.com/spreadsheets/d/1R1uvVDVMctyfUtvUQQDfRWswNFGjXa_NClBGUL5j-xs/edit?usp=sharing) is available as a shared document for you to copy and enjoy. You will need to add a few other formulas to cleanse the data and show resized images.