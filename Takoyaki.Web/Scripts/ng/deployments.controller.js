(function() {
	'use strict';

	angular
		.module('Environments')
		.controller('DeploymentsController', DeploymentsController);

	DeploymentsController.$inject = ['deploymentsService','$location'];

	function DeploymentsController(deploymentsService, $location) {
	    var vm = this;
	    var loadingText = "Loading...";
	    var compareText = "Compare";

	    vm.discrepancies;
	    vm.environments;
	    vm.totalProjects;
	    vm.projectsWithDiscrepancies;
	    vm.environmentOne;
	    vm.environmentTwo;

	    vm.buttonText = compareText;

	    vm.compare = compare;
	    vm.isLoading = false;

	    init();

	    function init() {
            deploymentsService.getAvailableEnvironments().then(function(data) {
                console.log(data);
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
            console.log(vm.environmentOne);
            deploymentsService.compareEnvironments(vm.environmentOne.Name, vm.environmentTwo.Name).then(function (data) {
                console.log(data);
                vm.discrepancies = data.Discrepancies;
                vm.projectsWithDiscrepancies = data.ProjectsWithDiscrepancies;
                vm.totalProjects = data.TotalProjects;
                $location.search('environmentOne', vm.environmentOne.Name);
                $location.search('environmentTwo', vm.environmentTwo.Name);
                loading(false);
            });
        }
	}
})();