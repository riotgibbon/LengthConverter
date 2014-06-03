(function () {
    var app = angular.module("converter", []);
    app.controller("ConverterController", ['$http', function ($http) {
        var converter = this;
        this.units = ["cm", "m"];
        $http.get('/Converter/Formats').success(function(data) {
            converter.units = data;
        });
        this.length = 1;
        this.inputFormat = this.units[0];
        this.outputFormat = this.units[0];
    }]);
})();