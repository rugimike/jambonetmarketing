'use strict';
var app = angular.module('myApp',[]);

app.directive('restrictInput', [function () {

    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var ele = element[0];
            var regex = RegExp(attrs.restrictInput);
            var value = ele.value;

            ele.addEventListener('keyup', function (e) {
                if (regex.test(ele.value)) {
                    value = ele.value;
                } else {
                    ele.value = value;
                }
            });
        }
    };
}]);

//var value = $input.val().replace(",",".");

app.directive('allowDecimalNumbers', function () {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            elm.on('keyup', function (event) {
                var $input = $(this);
                var value = $input.val().replace(",", ".");
                value = value.replace(/[^-0-9\.]/g, '')
                var findsDot = new RegExp(/\./g)
                var containsDot = value.match(findsDot)
                if (containsDot != null && ([46, 110, 190].indexOf(event.which) > -1)) {
                    event.preventDefault();
                    return false;
                }
                $input.val(value);
                if (event.which == 64 || event.which == 16) {
                    // numbers  
                    return false;
                } if ([8, 13, 27, 37, 38, 39, 40, 110].indexOf(event.which) > -1) {
                    // backspace, enter, escape, arrows  
                    return true;
                } else if (event.which >= 48 && event.which <= 57) {
                    // numbers  
                    return true;
                } else if (event.which >= 96 && event.which <= 105) {
                    // numpad number  
                    return true;
                } else if ([46, 110, 190].indexOf(event.which) > -1) {
                    // dot and numpad dot  
                    return true;
                } else {
                    event.preventDefault();
                    return false;
                }
            });
        }
    }
});

//escape the characters
app.directive('myDirective', function() {
    function link(scope, elem, attrs, ngModel) {
        ngModel.$parsers.push(function(viewValue) {
            var reg = /^[a-zA-Z0-9]*$/;
            // if view values matches regexp, update model value
            if (viewValue.match(reg)) {
                return viewValue;
            }
            // keep the model value as it is
            var transformedValue = ngModel.$modelValue;
            ngModel.$setViewValue(transformedValue);
            ngModel.$render();
            return transformedValue;
        });
    }

    return {
        restrict: 'A',
        require: 'ngModel',
        link: link
    };      
});