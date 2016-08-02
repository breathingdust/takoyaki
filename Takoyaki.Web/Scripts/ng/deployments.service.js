(function() {
    'use strict';

    angular
        .module('Environments')
        .factory('deploymentsService', deploymentsService);

    deploymentsService.$inject = ['$http'];

    function deploymentsService($http) {
        return {
            compareEnvironments: compareEnvironments,
            getAvailableEnvironments: getAvailableEnvironments
        };

        function compareEnvironments(env1, env2) {
            console.log(env1);
            var endpoint = "/octopus/compare?environmentOne=" + env1 + "&environmentTwo=" + env2;

            return $http({
                method: 'get',
                url: endpoint,
                }).then(onCompareEnvironmentsLoadedSuccess);
        }

        function onCompareEnvironmentsLoadedSuccess(response) {
            return response.data;
        }

        function getAvailableEnvironments() {
            var endpoint = "/octopus/availableEnvironments";

            return $http({
                method: 'get',
                url: endpoint,
            }).then(onGetAvailableEnvironmentsLoadedSuccess);
        }

        function onGetAvailableEnvironmentsLoadedSuccess(response) {
            return response.data;
        }
    }
})();