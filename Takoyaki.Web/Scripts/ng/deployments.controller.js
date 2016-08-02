(function() {
	'use strict';

	angular
		.module('Environments')
		.controller('DeploymentsController', DeploymentsController);

	DeploymentsController.$inject = ['deploymentsService','$location', '$interval'];

	function DeploymentsController(deploymentsService, $location, $interval) {
	    var vm = this;
	    var loadingText = "Loading...";
	    var compareText = "Compare";

	    vm.discrepancies;
	    vm.environments;
	    vm.totalProjects;
	    vm.projectsWithDiscrepancies;
	    vm.environmentOne;
	    vm.environmentTwo;
	    vm.deployingProjectId;
	    vm.deployingProjectStatus;

	    vm.buttonText = compareText;

	    vm.compare = compare;
	    vm.isLoading = false;
	    vm.deployForward = deployForward;
	    vm.forwardButtonName = forwardButtonName;
	    vm.deployBackward = deployBackward;

	    init();

	    function init() {
            deploymentsService.getAvailableEnvironments().then(function(data) {
                vm.environments = data;

                var queryParams = $location.search();

                if (queryParams.environmentOne) {
                    vm.environmentOne = vm.environments.find(function(e) {
                        return e.Name === queryParams.environmentOne;
                    });
                }

                if (queryParams.environmentTwo) {
                    vm.environmentTwo = vm.environments.find(function (e) {
                        return e.Name === queryParams.environmentTwo;
                    });
                }

                if (vm.environmentOne && vm.environmentTwo) {
                    compare();
                }
            });
	    }

        function loading(status) {
            if (status) {
                vm.isLoading = true;
                vm.buttonText = loadingText;

            } else {
                vm.isLoading = false;
                vm.buttonText = compareText;
            }
        }

	    function compare() {
	        loading(true);
            deploymentsService.compareEnvironments(vm.environmentOne.Name, vm.environmentTwo.Name).then(function (data) {
                vm.discrepancies = data.Discrepancies;
                vm.projectsWithDiscrepancies = data.ProjectsWithDiscrepancies;
                vm.totalProjects = data.TotalProjects;
                $location.search('environmentOne', vm.environmentOne.Name);
                $location.search('environmentTwo', vm.environmentTwo.Name);
                loading(false);
            });
	    }

	    function forwardButtonName(deployment) {
	        if (!vm.deployingProjectId || (vm.deployingProjectId && vm.deployingProjectId !== deployment.ProjectId)) {
	            return deployment.ReleaseOneName + " to " + vm.environmentTwo.Name;
            } else if (vm.deployingProjectId === deployment.ProjectId) {
	            return vm.deployingProjectStatus;
	        }
	    }

        function getStatus(taskId) {
            deploymentsService.getTaskStatus(taskId).then(function (response) {
                vm.deployingProjectStatus = response.data;
            });
        }

	    function deployForward(d) {
            //deploymentsService.deploy(d.ReleaseOneId, vm.environmentTwo.Id);
	        vm.deployingProjectId = d.ProjectId;
            vm.deployingProjectStatus = "Queued...";
            deploymentsService.deploy('Releases-3842', 'Environments-9').then(function(data) {
                var promise = $interval(getStatus, 250, 0, true, data);

                promise.then(function () {
                    if (vm.deployingProjectStatus !== "Queued" && vm.deployingProjectStatus !== "Executing")
                        stopInterval(result);
                    if (vm.deployingProjectStatus === "Success") {
                        vm.deployingProjectId = false;
                        vm.deployingProjectStatus = false;
                        vm.discrepancies.remove(d);
                    }
                    if (vm.deployingProjectStatus === "Failed") {
                        vm.deployingProjectId = false;
                        vm.deployingProjectStatus = false;
                    }
                });

                function stopInterval() {
                    $interval.cancel(promise);
                }
            });
        }

        function deployBackward(d) {
            //deploymentsService.deploy(d.ReleaseTwoId, vm.environmentOne.Id);
        }
	}
})();