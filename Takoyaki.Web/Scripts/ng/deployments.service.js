(function() {
    'use strict';

    angular
        .module('Environments')
        .factory('deploymentsService', deploymentsService);

    deploymentsService.$inject = ['$http'];

    function deploymentsService($http) {
        return {
            compareEnvironments: compareEnvironments,
            getAvailableEnvironments: getAvailableEnvironments,
            deploy: deploy,
            getTaskStatus: getTaskStatus
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

        function getTaskStatus(taskId) {
            var endpoint = "/octopus/getServerTaskStatus/?taskid=" + taskId;
            return $http({
                method: 'get',
                url: endpoint
            });
        }

        function deploy(releaseId,environmentId) {
            var endpoint = "/octopus/deploy";

            var body = {
                releaseId: releaseId,
                environmentId: environmentId
            };

            return $http({
                method: 'post',
                url: endpoint,
                headers: {
                    'Content-Type': 'application/json'
                },
                data: JSON.stringify(body)
            }).then(onDeploySuccess);
        }

        function onGetAvailableEnvironmentsLoadedSuccess(response) {
            return response.data;
        }

        function onDeploySuccess(response) {
            return response.data;
        }
    }
})();