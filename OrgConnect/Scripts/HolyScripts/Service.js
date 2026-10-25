app.service("OrgConnectService", function ($http) {

    this.GetMembersFunc = function () {
        return $http.get("/Main/GetMembers");
    };

    this.GetRegistrationDataFunc = function () {
        return $http.get("/Main/GetRegistrationData");
    };

    this.CreateMemberFunc = function (userData) {
        var response = $http({
            method: "post",
            url: "/Main/CreateMember",
            data: userData
        });
        return response;
    };

    this.UpdateMemberFunc = function (userData) {
        var response = $http({
            method: "post",
            url: "/Main/UpdateMember",
            data: userData
        });
        return response;
    };

    this.RemoveMemberFunc = function (memberID) {
        var response = $http({
            method: "post",
            url: "/Main/RemoveMember",
            params: {
                memberID: memberID
            }
        });
        return response;
    };
});
