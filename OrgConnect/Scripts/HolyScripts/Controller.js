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

            Swal.fire({ title: "Login Not Connected Yet", text: "This MP3 version saves registration records. Account login will be implemented in a later stage.", icon: "info", confirmButtonColor: "#0a4caf" });
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


        $scope.emailPattern = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$/;

        $scope.passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/;

        $scope.validatePasswordMatch = function (registrationForm) {
            if (!registrationForm || !registrationForm.confirmPassword) {
                return;
            }

            var passwordsMatch = !$scope.confirmPassword ||
                $scope.password === $scope.confirmPassword;

            registrationForm.confirmPassword.$setValidity(
                "passwordMatch",
                passwordsMatch
            );
        };


        // duplicate checks

        $scope.isDuplicateMemberID = function () {
            var enteredID = ($scope.memberID || "").toLowerCase();

            return $scope.userarray.some(function (user) {
                return user.MemberID.toLowerCase() === enteredID;
            });
        };

        $scope.isDuplicateUsername = function () {
            var enteredUsername = ($scope.username || "").toLowerCase();

            return $scope.userarray.some(function (user) {
                return (user.Username || "").toLowerCase() === enteredUsername;
            });
        };

        $scope.isDuplicateEmail = function () {
            var enteredEmail = ($scope.email || "").trim().toLowerCase();

            return $scope.userarray.some(function (user) {
                return (user.Email || "").trim().toLowerCase() === enteredEmail;
            });
        };

        $scope.isDuplicateContactNumber = function () {
            var enteredNumber = $scope.contactNumber || "";

            return $scope.userarray.some(function (user) {
                return user.ContactNumber === enteredNumber;
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

            if ($scope.isSaving) {
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
                Password: $scope.password,
                ConfirmPassword: $scope.confirmPassword,
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

        // clear registration form

        $scope.clearRegistrationFunc = function (registrationForm) {
            $scope.memberID = "";
            $scope.firstName = "";
            $scope.lastName = "";
            $scope.username = "";
            $scope.email = "";
            $scope.contactNumber = "";
            $scope.password = "";
            $scope.confirmPassword = "";
            $scope.committee = "";
            $scope.position = "";
            $scope.positionList = [];

            $scope.registrationSubmitted = false;

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


