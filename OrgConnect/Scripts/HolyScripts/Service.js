app.service("OrgConnectService", function() {

    this.getCommittees = function() {
        return [
            "Executive Committee",
            "Membership Committee",
            "Events Committee",
            "Finance Committee",
            "Marketing Committee",
            "Creatives Committee",
            "Documentation Committee"
        ];
    };

    this.getPositions = function(committee) {
        if (committee === "Executive Committee") {
            return [
                "President",
                "Vice President",
                "Secretary",
                "Treasurer"
            ];
        }

        if (committee) {
            return [
                "Committee Head",
                "Officer",
                "Member"
            ];
        }

        return [];
    };

});
