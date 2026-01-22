# Javascript

## Module Definition - IFEE
```javascript
var myModule = (function () {
    function myModule() {
        // ...
    }
    return myModule;
})();
```

## Module Definition - UMD Pattern
```javascript
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define([], factory);
    }
    else if (typeof exports === 'object' && typeof module === 'object') {
        module.exports = factory();
    }
    else {
        root.myModule = factory();
    }
})(window, function () {

    function myModule() {
        // ...
    }
    return myModule;
}); 
```