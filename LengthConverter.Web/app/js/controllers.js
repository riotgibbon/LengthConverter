(function () {
    var app = angular.module("converter", []);
    app.controller("ConverterController", function() {
        this.units = ["cm", "m"];
        this.length = 1;
        this.inputFormat = this.units[0];
        this.outputFormat = this.units[0];
    });
})();