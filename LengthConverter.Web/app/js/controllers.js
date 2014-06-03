(function () {
    var app = angular.module("converter", []);
    app.controller("ConverterController", ['$http', function ($http) {
        var converter = this;
        this.units = ["cm", "m"];
        this.result = "";
        $http.get('/Converter/Formats').success(function(data) {
            converter.units = data;
        });
        this.length = 1;
        this.inputFormat = this.units[0];
        this.outputFormat = this.units[0];

        this.getResult = function() {
            $http.get('/Converter/Length', { params: { length: converter.length, inputFormat: this.inputFormat, outputFormat: this.outputFormat } }).success(function (data) {
                converter.result = data;
            });
        };
        this.getResult();
    }]);
})();