app.controller('accountcontroller', ['$scope', '$log', function ($scope, $log) {
    var self = this;
    self.user = {};
    self.transaction = {};

    self.getservices = function () {
        $.ajax({
            type: "GET",
            url: "/Account/getservices",
            headers: {
                'AUTHTOKEN': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJPUkcwMDAxIiwibmJmIjoxNjAyNzgyNzU2LCJleHAiOjE2MDM2NDY3NTUsImlhdCI6MTYwMjc4Mjc1NiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoxODQ1NCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTg0NTQifQ.hLWYemj3hZ29FksqCcC6NlvpSRDxuxayfUXAzEEwzKY',
                'AUTHKEY': 'mysecretmysecretmysecretmysecretmysecretmysecret',
            },
            success: function (result) {
                console.log(result);
                $scope.$apply(function () {
                    self.services = JSON.parse(result);
                });   
            }
        });
    }

    self.getpaymenttypes = function () {
        $.ajax({
            type: "GET",
            url: "/Account/getpaymenttypes",
            headers: {
                'AUTHTOKEN': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJPUkcwMDAxIiwibmJmIjoxNjAyNzgyNzU2LCJleHAiOjE2MDM2NDY3NTUsImlhdCI6MTYwMjc4Mjc1NiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoxODQ1NCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTg0NTQifQ.hLWYemj3hZ29FksqCcC6NlvpSRDxuxayfUXAzEEwzKY',
                'AUTHKEY': 'mysecretmysecretmysecretmysecretmysecretmysecret',
            },
            success: function (result) {
                console.log(result);
                $scope.$apply(function () {
                    self.paytypes = JSON.parse(result);
                });   
            }
        });
    }


    self.getsupporters = function () {
        $.ajax({
            type: "GET",
            url: "/Account/getsupport",
            headers: {
                'AUTHTOKEN': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJPUkcwMDAxIiwibmJmIjoxNjAyNzgyNzU2LCJleHAiOjE2MDM2NDY3NTUsImlhdCI6MTYwMjc4Mjc1NiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoxODQ1NCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTg0NTQifQ.hLWYemj3hZ29FksqCcC6NlvpSRDxuxayfUXAzEEwzKY',
                'AUTHKEY': 'mysecretmysecretmysecretmysecretmysecretmysecret',
            },
            success: function (result) {
                console.log(result);
                $scope.$apply(function () {
                    self.supporters = JSON.parse(result);
                });
            }
        });
    }

    self.transaction = {};
    self.createTransaction = function () {
        console.log(self.transaction);
        $.ajax({
            url: "/account/createtrans",
            type: 'POST',
            headers: {
                'AUTHTOKEN': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJPUkcwMDAxIiwibmJmIjoxNjAyNzgyNzU2LCJleHAiOjE2MDM2NDY3NTUsImlhdCI6MTYwMjc4Mjc1NiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoxODQ1NCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTg0NTQifQ.hLWYemj3hZ29FksqCcC6NlvpSRDxuxayfUXAzEEwzKY',
                'AUTHKEY': 'mysecretmysecretmysecretmysecretmysecretmysecret',
            },
            contentType: 'application/json;charset=utf-8',
            data: JSON.stringify(self.transaction),
            success: function (result) {
                console.log(result);
                var resultdata = JSON.parse(result);
                alert(resultdata.message);
                $scope.$apply(function () {
                    self.transaction = {};
                });
            }
        });
    }



}]);


