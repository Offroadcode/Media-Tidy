angular.module("umbraco.resources").factory("orcMediaTidyDashboardApiResource", function ($http, $q) {

    var orcMediaTidyDashboardApiResource = {};

    // API Calls //////////////////////////////////////////////////

    orcMediaTidyDashboardApiResource.generateMediaReport = function() {
        return $http.get('/umbraco/backoffice/api/MediaTidy/GenerateMediaReport').then(function (response) {
            var responseData = response.data;
            if (typeof responseData === 'string') {
                responseData = JSON.parse(responseData);
            }

            if (responseData) {
                return responseData;
            }
        });
    };

    orcMediaTidyDashboardApiResource.archiveUnusedMedia = function() {
        return $http.get('/umbraco/backoffice/api/MediaTidy/ArchiveUnusedMedia').then(function (response) {
            var responseData = response.data;
            if (typeof responseData === 'string') {
                responseData = JSON.parse(responseData);
            }

            if (responseData) {
                return responseData;
            }
        });
    };

    // Helper Methods //////////////////////////////////////////////////
    getValidationMessages = function(responseData) {
        if (typeof responseData === 'string') {
            responseData = JSON.parse(responseData);
        }

        if (responseData) {
            return {
                validationMessages: responseData
            };
        }
    };

    return orcMediaTidyDashboardApiResource;
});

angular.module("umbraco").controller("orc.media.tidy.controller", function ($scope, contentTypeResource, notificationsService, orcMediaTidyDashboardApiResource) {

    // Initialization Methods //////////////////////////////////////////////////

    /**
     * @method init
     * @returns {void}
     * @description Triggered when the controller is loaded by a view to initialize the JS for the controller.
     */
    $scope.init = function() {
        $scope.mediaReport = "";
        $scope.archiveFolder = 0;
    };

    // Event Handler Methods ///////////////////////////////////////////////////

    /**
     * @method onUpdateStore
     * @param {Event} e The event object
     * @returns {void}
     * @description Triggered when the user clicks on the Update button
     */
    $scope.onGenerateMediaReport = function (e) {
        e.preventDefault()
        notificationsService.info("Started", "Running media audit.");
        return orcMediaTidyDashboardApiResource.generateMediaReport().then(function (response) {
            console.info(response);
            if(response.status != 200) {
                notificationsService.error("Error!", "There was an error generating the media report. Please try again or check the logs.");
            }
            else {
                notificationsService.success("Success!", "Please download your media audit.");
                $scope.mediaReport = response.data;
            }
        });
    };

    $scope.onArchiveUnusedMedia = function(e) {
        e.preventDefault();
        notificationsService.info("Started", "Archiving unused media; please be patient! We'll notify you when the task has finished.");
        return orcMediaTidyDashboardApiResource.archiveUnusedMedia().then(function (response) {
            console.info(response);
            if(response.status != 200) {
                notificationsService.error("Error!", "There was an error archiving your media. Please try again or check the logs.");
            }
            else {
                notificationsService.success("Success!", "You can now view your archived media in the Archive folder in the Media section.");
                $scope.archiveFolder = response.data;
            }
        });
    }

    // Call $scope.init() //////////////////////////////////////////////////////

    $scope.init();

});