(function() {
    'use strict';

    angular
        .module('Environments', ['ngRoute'])
        .config(routeConfig);

    routeConfig.$inject = ['$routeProvider'];

    function routeConfig($routeProvider) {
        $routeProvider
            .when('/deployments', {
                //templateUrl: 'scripts/ng/partials/deployments.html',
                title: 'deployments'
            })
            .otherwise({
                redirectTo: '/deployments'
            });
    }
})();
