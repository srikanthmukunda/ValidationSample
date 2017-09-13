var app = angular.module("myApp", ["ngRoute"]);
app.config(function($routeProvider) {
    $routeProvider
    //.when("/", {
    //    templateUrl : "GetAllTestCases"
    //})
    .when("/add", {
        templateUrl: "AddTestCase",
        controller : "addCtrl"
    })
    .when("/tst", {
        templateUrl: "AddTestCase",
        controller: "addCtrl"
    })
    //.when("/update/:testCaseId/testSuiteId/:testSuiteId/pipeLineStageId/:pipeLineStageId/testCaseTypeId/:testCaseTypeId/testCaseName/:testCaseName/description/:description/isActive/:isActive/isObsolete/:isObsolete/obsoleteReason/:obsoleteReason/createdDate/:createdDate/createdBy/:createdBy", {
    //    templateUrl: "UpdateTestCase",
    //    controller : "parisCtrl"
    //});
});
app.controller("addCtrl", function ($scope) {
    $scope.msg = "I love London";
    $scope.change = function () {
        document.getElementById('chkElgibility').disabled = false;
        if ($scope.testSuiteId == undefined)
        {
            alert($scope.errtstSuite);
            $scope.testSuiteId.focus();
            document.getElementById('chkElgibility').disabled = true;
        }

        if (($scope.status == '1') || ($scope.status == '3')) {
            document.getElementById('obsRsn').disabled = true;
        }
        else {
            document.getElementById('obsRsn').disabled = false;

        }
    }
    $scope.chkElgibility = function () {
        //debugger;
        $scope.click = true;
        
        if($scope.testSuiteId==undefined)
        {
            $scope.errtstSuite = "Select Testsuite";
            $scope.click = false;
        }
    $scope.runFunctionAgain = true;
    $scope.$on('$routeChangeStart', function (event, next, current) {
        scope.runFunctionAgain = true;
    });
        
    }
});

app.controller("parisCtrl", function ($scope, $routeParams) {
    $scope.msg = "I love Paris";
});
app.directive('testJquery', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).parallex();
        }
    };
});