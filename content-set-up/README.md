# Content set up

This section will guide you on setting up the content to try this entire scenario on your own. You can sign up for a [trial of Umbraco Heartcore](https://umbraco.com/try-umbraco-heartcore) to try this out.

### Document Types
There are a few document types needed to support the website. But to be succinct, in this article, you will only concentrate on two document types used for managing products.
| Document Type | Group | Property | Editor |
|--------------------|---------|---------------------|-----------------|
| Product Collection | Content | Title | Textstring |
| | | Sub Title | Textstring |
| Product | Content | Title | Textstring |
| | | Price | Textstring |
| | | Description | Richtext editor |
| | | Image | Media picker |
| | | Quantity | Textstring |
| | | Is on special today | True/false |

*Note that all fields are mandatory.*

**Relationships**

`Product Collection` document type is allowed in the root and `Product` document types are allowed as child nodes of `Product Collection`. It would be easy starting by creating `Document Type Collection` with `Product Collection` as the parent and `Product` as the item.

Once you have done creating the content types, you can request the content type definition for the `product` with the following request:
```
curl --location --request GET 'https://api.umbraco.io/content/type/product' \
  --header 'Authorization: Basic YOURACCESSKEYHERE' \
  --header 'umb-project-alias: your-project-alias' \
```
In response, you will get the JSON payload with the properties of the `product` content type.

### Content Population
Once you have got the content types set up manually, you can use the Content Management API to [create content](https://our.umbraco.com/documentation/Umbraco-Heartcore/API-Documentation/Content-Management/content/#create-content). Here’s an example where we create a product:
```
curl --location --request POST 'https://api.umbraco.io/content/' \
--header 'Authorization: Basic YOURACCESSKEYHERE' \
--header 'umb-project-alias: your-project-alias' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": {
        "$invariant": "Classic Lamington"
    },
    "contentTypeAlias": "product",
    "parentId": "guid-of-the-product-collection",
    "sortOrder": 0,
    "title": {
        "$invariant": "Classic Lamington"
    },
    "price": {
        "$invariant": "2.50"
    },
    "image": {
        "$invariant": "umb://media/f1485357ba7b4d8abea23c0789ba572c"
    },
    "description": {
        "$invariant": "<p>The classics in a pack of six.</p>"
    },
    "quantity": {
        "$invariant": 38
    },
    "isOnSpecialToday": {
        "$invariant": true
    }
}'
```
You should get a `201 Created` response back if your request is successful.

The next step is to publish that content item.
``` 
curl --location --request PUT 'https://api.umbraco.io/content/guid-returned-from-previous-action/publish' \
  --header 'Authorization: Basic YOURACCESSKEYHERE' \
  --header 'umb-project-alias: your-project-alias' \
```
You should get `200 Success` response when your item is successfully published.

If you are using Postman to create content [this Postman collection](https://www.getpostman.com/collections/72e3c82c0d50fa986ecd) will help you get on the way. Note that the images referred to in the scripts were uploaded manually and then referenced. The images can be found in this folder.

In this manner, we have published the following products.![Products](https://lh3.googleusercontent.com/12Bcd692Uy7fyxxAvNN6e3ofgzT-HgXmJFXpnU63_D4-oizYPrkF-d6ROlIoxeSCDffGWmBodLFrFwSqBPaYLz865izSk723L2SUmO1YUzKbL9b7i3Fk8vkrJRiG_0EAx_ErFcufkP9g7GHmYcqjUV_ImUtH_yJxwbXpw3ttfL2yo7IsZQHoRXqBjhhM52ZdOc5vOkm9o0WNw5aCj_IY2KRm6n8fNRq3B2LDeH-xsqVU89d3YIT_DuQYgTkPguGk5de53WVIRgxdc-v4dJxTfkLl3oDuowNgj0yulCT9CUffxwPBcfdXm1KBL0qI7XygXkhe1BAExFhayUJk_DtINsLiGj6p2qUVhyBhSC54JuL6zJNZDVR31jO-8UGp8IvKw07_jkzz9SASRfzdAJgOUdQY28GHg9BTSQEDhoRlegvYmdDMBACH3LDJCm7kPQlPj6tgryZr7_GBPpPR0mzfDWaWT-vI2FuD1p2U6vJGouPcC7Xd59Oumcw2IeArWX029vMDqK3Qm8cv6Zm1H88o6ywNKTB-wIuuz3C4L7ffEs3v8ao6XcsNo7j-hlftvq91J_OPLEKHNXiUs8c62AiTEXUGuHnqj0FS0RO7Z4Yfa1N0BjfTuvkghsyG4gO4F6HcRKnyHw0P9a4tZAfl3psDdWQqGM0Xf4rFri4XFGCXI__jlH_SwzM5T75AFGj9dAaObVGmMkwbr-TU-KodVeOAczuc_N0jFJ1lb-aVYuhCs3EmkwZ1NA=w1920-h674-no)