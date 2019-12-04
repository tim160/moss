(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCompany',
            ['$scope', '$filter', '$location', 'NewCompanyService', NewCompany]);

    function NewCompany($scope, $filter, $location, NewCompanyService) {

        $scope.model = {
            Data: $filter('parseUrl')($location.$$absUrl, 'data'),
            InvitationCode: '',
            CompanyName: '',
            PrimaryLocationUp: '',
            NumberEmployees: 0,
            NumberOfNonEmployees: 0,
            NumberOfClients: 0,
            Language: 1,
            Role: 5,
            FirstName: '',
            LastName: '',
            Email: '',
            Title: '',
            Department: '',
            Amount: 0,
            CreditCardNumber: '',
            NameonCard: '',
            ExpiryDate: 'mm',
            SelectedYear: 'yyyy',
            CSV: 0,
        };

        $scope.years = [];
        for (var i = new Date().getFullYear(); i <= 2030; i++) {
            $scope.years.push(i);
        }

        $scope.refresh = function () {
            NewCompanyService.calc($scope.model, function (data) {
                $scope.model = data.Model;
            });
        };

        $scope.register = function (form) {
            form.$submitted = true;
            if (form.$valid) {
                NewCompanyService.create($scope.model, function (data) {
                    if (!data.Result) {
                        alert(data.Message);
                    } else {
                        $.ajax({
                            method: 'POST',
                            url: '/New/CreateCompany',
                            data: {
                                code: $scope.model.InvitationCode.trim(),
                                location: $scope.model.PrimaryLocationUp.trim(),
                                departments: $scope.model.Department.trim(),
                                company_name: $scope.model.CompanyName.trim(),
                                number: $scope.model.NumberEmployees,
                                numberN: $scope.model.NumberOfNonEmployees,
                                numberC: $scope.model.NumberOfClients,
                                first: $scope.model.FirstName.trim(),
                                last: $scope.model.LastName.trim(),
                                email: $scope.model.Email.trim(),
                                title: $scope.model.Title.trim(),
                                csv: $scope.model.CSV.trim(),
                                cardname: $scope.model.NameonCard.trim(),
                                cardnumber: $scope.model.CreditCardNumber.trim(),
                                selectedMonth: $scope.model.ExpiryDate,
                                selectedYear: $scope.model.SelectedYear,
                                amount: $scope.model.Amount,
                                description: '',
                                contractors_number: $scope.model.NumberOfNonEmployees,
                                customers_number: $scope.model.NumberOfClients,
                            }
                        }).done(function (data) {
                            if (data !== 'completed') {
                            } else {
                                window.location.href = '/new/success?show=1';
                            }
                        }).fail(function (error) {
                            console.log(error);
                        });
                    }
                });

            } else {
                console.log(form);
            }
        };
    }
}());
