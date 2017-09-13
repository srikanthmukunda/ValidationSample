var app = angular.module('myApp', []);
app.controller('InfinityScrollController', ['$scope', '$http', function ($scope, $http) {
    $scope.CurrentPage = 1;
    $scope.TotalPage = 0;
    $scope.TestcaseLogList = [];

    function GetTestccaseLogData(page) {
        $scope.IsLoading = true;
        $http({
            method: 'GET',
            url: '/main/GetTestcaseLogData',
            params: { 'page': page }
        }).then(function (response) {
            angular.forEach(response.data.List, function (value) {
                
                $scope.TestcaseLogList.push(value);
            });
            $scope.TotalPage = response.data.totalPage;
            $scope.IsLoading = false;
        }, function () {
            $scope.IsLoading = false;
        })
    }
    

    GetTestccaseLogData($scope.CurrentPage);

    $scope.NextPage = function () {
        if ($scope.CurrentPage < $scope.TotalPage) {
            $scope.CurrentPage += 1;
            GetTestccaseLogData($scope.CurrentPage);
        }
    }
}]);

//directive
app.directive('infinityscroll', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.bind('scroll', function () {
                if ((element[0].scrollTop + element[0].offsetHeight) == element[0].scrollHeight) {
                    //scroll reach to end
                    scope.$apply(attrs.infinityscroll)
                }
            });
        }
    }
});