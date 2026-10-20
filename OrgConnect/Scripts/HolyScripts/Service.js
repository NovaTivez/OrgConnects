app.service("OrgConnectService", function ($http, $httpParamSerializerJQLike) {
    this.CheckDatabaseFunc = function () {
        return $http.get("/Main/CheckDatabase");
    };

    this.GetRegistrationDataFunc = function () {
        return $http.get("/Main/GetRegistrationData");
    };

    this.CreateMemberFunc = function (userData) {
        var token = document.querySelector('input[name="__RequestVerificationToken"]');
        var requestData = angular.extend({}, userData, {
            __RequestVerificationToken: token ? token.value : ""
        });

        var response = $http({
            method: "post",
            url: "/Main/CreateMember",
            data: $httpParamSerializerJQLike(requestData),
            headers: { "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8" }
        });
        return response;
    };
    this.UpdateMemberFunc = function (userData) {
        var token = document.querySelector('input[name="__RequestVerificationToken"]');
        var requestData = angular.extend({}, userData, {
            __RequestVerificationToken: token ? token.value : ""
        });

        var response = $http({
            method: "post",
            url: "/Main/UpdateMember",
            data: $httpParamSerializerJQLike(requestData),
            headers: { "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8" }
        });
        return response;
    };
    this.RemoveMemberFunc = function (memberID) {
        var token = document.querySelector('input[name="__RequestVerificationToken"]');
        var requestData = angular.extend({}, { memberID: memberID }, {
            __RequestVerificationToken: token ? token.value : ""
        });

        var response = $http({
            method: "post",
            url: "/Main/RemoveMember",
            data: $httpParamSerializerJQLike(requestData),
            headers: { "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8" }
        });
        return response;
    };
});
