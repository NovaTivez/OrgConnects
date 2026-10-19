app.service("OrgConnectService", function ($http) {
    this.CheckDatabaseFunc = function () {
        return $http.get("/Main/CheckDatabase");
    };
});