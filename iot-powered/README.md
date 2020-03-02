# IoT Powered Display Screen implementation

-   This display screen runs off a Raspberry Pi running Raspian Buster. You can follow this article on how to [set up a Kiosk using Chromium](https://pimylifeup.com/raspberry-pi-kiosk/).  
-   Then you will set up a static HTML page served using [GitHub pages](https://pages.github.com/).
-   The [static page](http://iot.heartcore.lordlamington.com/display/?language=en-AU) will load and the Heartcore content will be pulled via a JQuery AJAX request from the client browser. The AJAX request is crafted so: 
```javascript
$.ajax({
  type: "GET",
  url: "https://cdn.umbraco.io/content/type?contentType=product&page=1&pageSize=3",
  dataType: "json",
  headers: {
    "Accept-Language": "en-AU",
    "umb-project-alias": "your-project-alias"
  },
  success: processData,
  error: function() {
    showAlert("JSON call to CaaS failed - retrieve slides");
  }
});
```  
The complete code for the implementation of the static page can be found here - [https://github.com/emmanueltissera/umbraco-heartcore-lamington/tree/master/display](https://github.com/emmanueltissera/umbraco-heartcore-lamington/tree/master/display)
