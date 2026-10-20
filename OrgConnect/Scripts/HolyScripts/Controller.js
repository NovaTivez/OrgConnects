app.controller("OrgConnectController",
    function ($scope, OrgConnectService) {

        // login

        $scope.loginSubmitted = false;

        $scope.loginFunc = function (loginForm) {
            $scope.loginSubmitted = true;

            if (loginForm.$invalid) {
                Swal.fire({ title: "Login Failed", text: "Please enter a valid username and password.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            Swal.fire({ title: "Login Not Connected Yet", text: "Password setup and account login are deferred. This MP3 version saves member profiles and disabled account records only.", icon: "info", confirmButtonColor: "#0a4caf" });
        };

        $scope.clearLoginFunc = function (loginForm) {
            $scope.loginUsername = "";
            $scope.loginPassword = "";
            $scope.loginSubmitted = false;

            if (loginForm) {
                loginForm.$setPristine();
                loginForm.$setUntouched();
            }
        };

        $scope.registrationRedirectFunc = function () {
            window.location.href = "/Main/RegistrationPage";
        };


        // registration data

        $scope.userarray = [];
        $scope.committeeList = [];
        $scope.positionList = [];
        $scope.registrationOptions = [];
        $scope.isSaving = false;
        $scope.optionsLoading = false;
        $scope.optionsError = "";
        $scope.membersLoading = false;
        $scope.membersError = "";
        $scope.deletingID = null;
        $scope.editingDatabaseID = 0;
        $scope.loadRegistrationDataFunc = function () {
            $scope.optionsLoading = true;
            $scope.optionsError = "";
            $scope.committeeList = [];
            $scope.positionList = [];
            $scope.registrationOptions = [];
            OrgConnectService.GetRegistrationDataFunc().then(function (response) {
                $scope.optionsLoading = false;
                if (!response.data.Success) {
                    $scope.optionsError = response.data.Message;
                    return;
                }
                $scope.registrationOptions = response.data.Options;
                angular.forEach($scope.registrationOptions, function (item) {
                    if ($scope.committeeList.indexOf(item.Committee) === -1) {
                        $scope.committeeList.push(item.Committee);
                    }
                });
                if (!$scope.committeeList.length) {
                    $scope.optionsError = "No available committee-position assignments. Run the initial-data SQL first.";
                }
            }, function () {
                $scope.optionsLoading = false;
                $scope.optionsError = "Could not load committees and positions. Check the database connection.";
            });
        };

        $scope.registrationSubmitted = false;
        $scope.isEditing = false;
        $scope.editingIndex = -1;


        $scope.emailPattern = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$/;

        // duplicate checks

        $scope.isDuplicateMemberID = function () {
            var enteredID = ($scope.memberID || "").toLowerCase();

            return $scope.userarray.some(function (user, index) {
                return user.DatabaseID !== $scope.editingDatabaseID &&
                    user.MemberID.toLowerCase() === enteredID;
            });
        };

        $scope.isDuplicateUsername = function () {
            var enteredUsername = ($scope.username || "").toLowerCase();

            return $scope.userarray.some(function (user, index) {
                return user.DatabaseID !== $scope.editingDatabaseID &&
                    (user.Username || "").toLowerCase() === enteredUsername;
            });
        };

        $scope.isDuplicateEmail = function () {
            var enteredEmail = ($scope.email || "").trim().toLowerCase();

            return $scope.userarray.some(function (user, index) {
                return user.DatabaseID !== $scope.editingDatabaseID &&
                    (user.Email || "").trim().toLowerCase() === enteredEmail;
            });
        };

        $scope.isDuplicateContactNumber = function () {
            var enteredNumber = $scope.contactNumber || "";

            return $scope.userarray.some(function (user, index) {
                return user.DatabaseID !== $scope.editingDatabaseID &&
                    user.ContactNumber === enteredNumber;
            });
        };

        // removes letters and symbols from the contact number
        $scope.numbersOnlyFunc = function () {
            $scope.contactNumber = ($scope.contactNumber || "")
                .replace(/[^0-9]/g, "")
                .slice(0, 11);
        };

        $scope.committeeChangedFunc = function () {
            $scope.position = "";
            $scope.positionList = [];
            angular.forEach($scope.registrationOptions, function (item) {
                if (item.Committee === $scope.committee && $scope.positionList.indexOf(item.Position) === -1) {
                    $scope.positionList.push(item.Position);
                }
            });
        };

        // create

        $scope.registrationFunc = function (registrationForm) {
            $scope.registrationSubmitted = true;

            if (registrationForm.$invalid) {
                Swal.fire({ title: "Invalid Information", text: "Please correct the highlighted fields.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }


            if ($scope.isDuplicateMemberID()) {
                Swal.fire({ title: "Duplicate Member ID", text: "The entered Member ID already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateUsername()) {
                Swal.fire({ title: "Duplicate Username", text: "The entered username already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateEmail()) {
                Swal.fire({ title: "Duplicate Email", text: "The entered email address already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateContactNumber()) {
                Swal.fire({ title: "Duplicate Contact Number", text: "The entered contact number already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }



            if ($scope.isSaving || $scope.deletingID !== null || $scope.membersLoading) {
                return;
            }
            $scope.isSaving = true;
            var userData = {
                MemberID: $scope.memberID.toUpperCase(),
                FName: $scope.firstName,
                LName: $scope.lastName,
                Username: $scope.username,
                Email: $scope.email.trim().toLowerCase(),
                ContactNumber: $scope.contactNumber,

                Committee: $scope.committee,
                Position: $scope.position
            };
            OrgConnectService.CreateMemberFunc(userData).then(function (returnedData) {
                $scope.isSaving = false;
                var result = returnedData.data;
                if (!result.Success) {
                    Swal.fire({ title: result.Title || "Registration Failed", text: result.Message, icon: "error", confirmButtonColor: "#0a4caf" });
                    return;
                }
                $scope.userarray.push(result.Member);
                Swal.fire({ title: "Registration Successful", text: "The organization member was registered.", icon: "success", confirmButtonColor: "#0a4caf" });
                $scope.clearRegistrationFunc(registrationForm);
            }, function () {
                $scope.isSaving = false;
                Swal.fire({ title: "Registration Failed", text: "The request failed. Refresh the page and check the database connection before trying again.", icon: "error", confirmButtonColor: "#0a4caf" });
            });
        };

        $scope.loadMembersFunc = function () {
            $scope.membersLoading = true;
            $scope.membersError = "";
            OrgConnectService.GetMembersFunc().then(function (response) {
                $scope.membersLoading = false;
                if (!response.data.Success) {
                    $scope.membersError = response.data.Message;
                    return;
                }
                $scope.userarray = response.data.Members;
                $scope.clearRegistrationFunc();
            }, function () {
                $scope.membersLoading = false;
                $scope.membersError = "The saved records could not be loaded. Check the database connection.";
            });
        };

        $scope.editFunc = function (userindex) {
            if ($scope.isSaving || $scope.optionsLoading || $scope.deletingID !== null || $scope.membersLoading) {
                return;
            }
            var user = $scope.userarray[userindex];
            $scope.editingIndex = userindex;
            $scope.editingDatabaseID = user.DatabaseID;
            $scope.isEditing = true;
            $scope.memberID = user.MemberID;
            $scope.firstName = user.FName;
            $scope.lastName = user.LName;
            $scope.username = user.Username;
            $scope.email = user.Email;
            $scope.contactNumber = user.ContactNumber;


            $scope.committee = user.Committee;
            $scope.committeeChangedFunc();
            $scope.position = $scope.positionList.indexOf(user.Position) >= 0 ? user.Position : "";
            setTimeout(function () {
                if (window.M && window.M.updateTextFields) {
                    window.M.updateTextFields();
                }
            }, 0);
            window.scrollTo({ top: 0, behavior: "smooth" });
        };

        $scope.updateFunc = function (registrationForm) {
            $scope.registrationSubmitted = true;

            if (registrationForm.$invalid) {
                Swal.fire({ title: "Invalid Information", text: "Please correct the highlighted fields.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }


            if ($scope.isDuplicateMemberID()) {
                Swal.fire({ title: "Duplicate Member ID", text: "The entered Member ID already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateUsername()) {
                Swal.fire({ title: "Duplicate Username", text: "The entered username already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateEmail()) {
                Swal.fire({ title: "Duplicate Email", text: "The entered email address already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateContactNumber()) {
                Swal.fire({ title: "Duplicate Contact Number", text: "The entered contact number already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }



            if ($scope.isSaving || $scope.deletingID !== null) {
                return;
            }
            $scope.isSaving = true;
            var userData = {
                DatabaseID: $scope.editingDatabaseID,
                MemberID: $scope.memberID.toUpperCase(),
                FName: $scope.firstName,
                LName: $scope.lastName,
                Username: $scope.username,
                Email: $scope.email.trim().toLowerCase(),
                ContactNumber: $scope.contactNumber,

                Committee: $scope.committee,
                Position: $scope.position
            };
            OrgConnectService.UpdateMemberFunc(userData).then(function (returnedData) {
                $scope.isSaving = false;
                var result = returnedData.data;
                if (!result.Success) {
                    Swal.fire({ title: result.Title || "Update Failed", text: result.Message, icon: "error", confirmButtonColor: "#0a4caf" });
                    return;
                }
                angular.forEach($scope.userarray, function (user, index) {
                    if (user.DatabaseID === result.Member.DatabaseID) {
                        $scope.userarray[index] = result.Member;
                    }
                });
                Swal.fire({ title: "Update Successful", text: "The member information was updated.", icon: "success", confirmButtonColor: "#0a4caf" });
                $scope.clearRegistrationFunc(registrationForm);
            }, function () {
                $scope.isSaving = false;
                Swal.fire({ title: "Update Failed", text: "The request failed. Refresh the page and check the database connection before trying again.", icon: "error", confirmButtonColor: "#0a4caf" });
            });
        };

        $scope.deleteFunc = function (userindex) {
            if ($scope.isSaving || $scope.deletingID !== null || $scope.membersLoading) {
                return;
            }
            var user = $scope.userarray[userindex];
            // Set before opening the dialog to prevent overlapping delete dialogs/requests.
            $scope.deletingID = user.DatabaseID;
            Swal.fire({
                title: "Delete Member?",
                text: "Remove " + user.FName + " " + user.LName + "? This also deletes the linked account from the database.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#c62828",
                cancelButtonColor: "#616161",
                confirmButtonText: "Yes, delete"
            }).then(function (result) {
                if (!result.isConfirmed) {
                    $scope.$applyAsync(function () { $scope.deletingID = null; });
                    return;
                }
                OrgConnectService.RemoveMemberFunc(user.DatabaseID).then(function (returnedData) {
                    $scope.deletingID = null;
                    var response = returnedData.data;
                    if (!response.Success) {
                        Swal.fire({ title: response.Title || "Delete Failed", text: response.Message, icon: "error", confirmButtonColor: "#0a4caf" });
                        return;
                    }
                    $scope.userarray = $scope.userarray.filter(function (item) {
                        return item.DatabaseID !== user.DatabaseID;
                    });
                    if ($scope.editingDatabaseID === user.DatabaseID) {
                        $scope.clearRegistrationFunc();
                    }
                    Swal.fire({ title: "Deleted", text: "The member record was removed.", icon: "success", confirmButtonColor: "#0a4caf" });
                }, function () {
                    $scope.deletingID = null;
                    Swal.fire({ title: "Delete Failed", text: "The request failed. Refresh the page and check the database connection before trying again.", icon: "error", confirmButtonColor: "#0a4caf" });
                });
            });
        };

        $scope.cancelEditFunc = function (registrationForm) {
            $scope.clearRegistrationFunc(registrationForm);
        };

        // clear registration form

        $scope.clearRegistrationFunc = function (registrationForm) {
            $scope.memberID = "";
            $scope.firstName = "";
            $scope.lastName = "";
            $scope.username = "";
            $scope.email = "";
            $scope.contactNumber = "";


            $scope.committee = "";
            $scope.position = "";
            $scope.positionList = [];

            $scope.registrationSubmitted = false;
            $scope.isEditing = false;
            $scope.editingIndex = -1;
            $scope.editingDatabaseID = 0;

            if (registrationForm) {
                registrationForm.$setPristine();
                registrationForm.$setUntouched();
            }

            setTimeout(function () {
                if (window.M && window.M.updateTextFields) {
                    window.M.updateTextFields();
                }
            }, 0);
        };

        $scope.databaseStatus = "Not checked";
        $scope.databaseResult = null;
        $scope.checkDatabaseFunc = function () {
            $scope.databaseStatus = "Checking connection...";
            $scope.databaseResult = null;
            OrgConnectService.CheckDatabaseFunc().then(function (returnedData) {
                $scope.databaseResult = returnedData.data;
                $scope.databaseStatus = returnedData.data.Success
                    ? "Database connected successfully."
                    : returnedData.data.Message;
            }, function () {
                $scope.databaseStatus = "Database check failed. Check the connection settings and database setup.";
            });
        };

    });