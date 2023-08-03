$(document).ready(function () {
    loadData();
    
    $('#FirstName').on("blur", (function () {
        let firstNameregex = /^[A-Za-z]{2,10}$/;
        let checkFirstname = $('#FirstName').val()
        if (firstNameregex.test(checkFirstname)) {
            $('#FirstName').css('border-color', 'lightgrey');
            $('#ErrFirstName').hide();
        }
        else {
            
            $('#ErrFirstName').show();
            $('#ErrFirstName').html("Enter valid first name");
            isValid = false;
        }
    }));
    $('#LastName').on("blur", (function () {
        let firstNameregex = /^[A-Za-z]{2,10}$/;
        let checkFirstname = $('#FirstName').val()
        if (firstNameregex.test(checkFirstname)) {
            $('#LastName').css('border-color', 'lightgrey');
            $('#ErrLastName').hide();
        }
        else {
            $('#ErrLastName').show();
            $('#ErrLastName').html("Enter valid last name");
            isValid = false;
        }
    }));
   
    $('#EmailID').on("blur", (function () {

        let emailregex =
            /^([_\-\.0-9a-zA-Z]+)@([_\-\.0-9a-zA-Z]+)\.([a-zA-Z]){2,7}$/;
        let checkEmail = $('#EmailID').val()
        
        if (emailregex.test(checkEmail)) {
            $('#EmailID').css('border-color', 'lightgrey');
            $('#ErrEmailID').hide();
        }
        else {
            $('#ErrEmailID').show();
            $('#ErrEmailID').html("Enter valid Email Id");
            isValid = false;
        }
    }));

    $('#Password').on("blur", (function () {
        let passwordRegex = /^(?=.*[!@#$%^&*]).{5,}$/;
    let checkPassword = $('#Password').val();

        if (passwordRegex.test(checkPassword)) {
           
            $('#ErrPassword').hide();


        } else {
            $('#ErrPassword').show();
            $('#ErrPassword').html("Password must contain min. 5 character and 1 unique symbol");
            isValid = false;
        }
    }));
    
});


function loadData() {
    $.ajax({
        url: "/Home/List",
        type: "GET",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<tr>';
                html += '<td>' + item.firstName + ' ' + item.lastName + '</td>';
                html += '<td>' + item.code + '</td>';
                html += '<td>' + item.emailID + '</td>';
                html += '<td>' + item.mobile + '</td>';
                html += '<td>' + item.salary + '</td>';
                html += '<td><a href="#" onclick="return getbyID(' + item.employeeID + ')"><i class="fas fa-pen"></i></a></td>';
                html += '<td><a href="#" onclick="Delete(' + item.employeeID + ')"><i class="fas fa-trash-alt"></a></td>';
                html += '</tr>';
            });
            $('.tbody').html(html);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}


function CheckButton() {
    //var res = validateSignIn();
    //if (res == false) {
    //    return false;
    //}

    var signupObj = {

        emailID: $('#EmailID').val(),
        password: $('#Password').val()

    };
    $.ajax({
        url: "/Home/CheckSignIn",
        data: signupObj,
        type: "POST",
        
        success: function (result) {
            console.log(result);
            if (result > 0 ) {
                if (window.location.pathname === '/Home/Privacy') {
                    window.location.href = '/Home/Index';
                }
                else {
                    window.location.href = window.location.pathname;
                }
                $('#Password').val("");
                $('#EmailID').val("");
            }
            else {
                alert("Invalid email or password");
            }
            
            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}


function AddAcc() {
    var res = validateSignUp();
    if (res == false)
    {
        return false;
    }
    
    console.log("add");

    var signupObj = {
        firstName: $('#FirstName').val(),
        lastName: $('#LastName').val(),
        emailID: $('#EmailID').val(),
        password: $('#Password').val()
        
    };

    $.ajax({
        url: "/Home/AddAcc",
        data: signupObj,
        type: "POST",
        //beforeSend: function (xhr) {
        //    xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        //},
       success: function (result) {
           if (result === -1) {
               // Display error message
               $('#ErrEmailID').show();
               $('#ErrEmailID').html("Enter already exists");
               //alert("This email already exists.");
           } else {
               // Display success message
               alert("Employee added successfully.");
               $('#FirstName').val("");
               $('#LastName').val("");
               $('#EmailID').val("");
               $('#Password').val("");
               window.location.href = "/Home/signup"
               //return RedirectToAction("Privacy", "Home");
           }
           
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}


function Add() {
    $('#myModal').modal('show');
    var res = validate();
    if (res == false) {
        return false;
    }

    var empObj = {
        employeeID: $('#EmployeeID').val(),
        firstName: $('#FirstName').val(),
        lastName: $('#LastName').val(),
        code: $('#Code').val(),
        emailID: $('#EmailID').val(),
        mobile: $('#Mobile').val(),
        salary: $('#Salary').val()
    };

    $.ajax({
        url: "/Home/Add",
        data: empObj,
        type: "POST",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (result) {
            if (result === -1) {
                // Display error message
                alert("This email already exists.");
            } else {
                // Display success message
                alert("Employee added successfully.");

                $(".modal-backdrop.show").css("opacity", "0");
                $(".modal-backdrop").css("position", "static");
                $(".modal-backdrop").css("background-color", "#FFF");
                loadData();
                $('#editModalLabel').hide();
                $('#addModalLabel').show();
                $('#btnUpdate').hide();
                $('#btnAdd').show();
                $('#myModal').modal('hide');
                
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function clearTextBox() {
    $('#EmployeeID').val("");
    $('#FirstName').val("");
    $('#LastName').val("");
    $('#Code').val("");
    $('#EmailID').val("");
    $('#Mobile').val("");
    $('#Salary').val("");

}
function getbyID(EmpID) {
    
    $.ajax({
        

        url: "/Home/getbyID/" + EmpID,
        type: "GET",
        dataType: "json",
        success: function (result) {
            $('#EmployeeID').val(result.employeeID);
            $('#FirstName').val(result.firstName);
            $('#LastName').val(result.lastName);
            $('#Code').val(result.code);
            $('#EmailID').val(result.emailID);
            $('#Mobile').val(result.mobile);
            $('#Salary').val(result.salary);

            
            $('#myModal').modal('show');
            $('#editModalLabel').show();
            $('#addModalLabel').hide();
            $('#btnUpdate').show();
            $('#btnAdd').hide();
            
            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}


function Delete(id) {

    var ans = confirm("are you sure you want to delete ?");
    if (ans) {
        console.log(id);
        $.ajax({
            url: "/home/delete/" + id,
            type: "delete",  
            //datatype: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                loadData();
            },
            error: function (errormessage) {
                alert(errormessage.responsetext);
            }
        });
    }
}

function validateSignUp() {
    var isValid = true;
    if ($('#FirstName').val().length < 2) {
        $('#ErrFirstName').show();
        $('#ErrFirstName').html("Enter valid first name");
        isValid = false;
    }
    else {
        $('#FirstName').css('border-color', 'lightgrey');
        $('#ErrFirstName').hide();
    }

    if ($('#LastName').val().length < 2) {
        $('#ErrLastName').show();
        $('#ErrLastName').html("Enter valid last name");
        isValid = false;
    }
    else {
        $('#LastName').css('border-color', 'lightgrey');
        $('#ErrLastName').hide();
    }
    let emailregex = /^([_\-\.0-9a-zA-Z]+)@([_\-\.0-9a-zA-Z]+)\.([a-zA-Z]){2,7}$/;
    let checkEmail = $('#EmailID').val();

    if (emailregex.test(checkEmail)) {
        $('#EmailID').css('border-color', 'lightgrey');
        $('#ErrEmailID').hide();


    } else {
        $('#ErrEmailID').show();
        $('#ErrEmailID').text("Enter valid Email Id");
        isValid = false;
    }

    let passwordRegex = /^(?=.*[!@#$%^&*]).{5,}$/;
    let checkPassword = $('#Password').val();

    if (passwordRegex.test(checkPassword)) {
        $('#Password').css('border-color', 'lightgrey');
        $('#ErrPassword').hide();

    } else {
        $('#ErrPassword').show();
        $('#ErrPassword').html("Password must contain min. 5 character and 1 unique symbol");
        isValid = false;
    }
}
function validateSignIn() {
    var isValid = true;
    let emailregex = /^([_\-\.0-9a-zA-Z]+)@([_\-\.0-9a-zA-Z]+)\.([a-zA-Z]){2,7}$/;
    let checkEmail = $('#EmailID').val();

    if (emailregex.test(checkEmail)) {
        $('#EmailID').css('border-color', 'lightgrey');
        $('#ErrEmailID').hide();


    } else {
        $('#ErrEmailID').show();
        $('#ErrEmailID').text("Enter valid Email Id");
        isValid = false;
    }

    let passwordregex = /^(?=.*[!@#$%^&*]).{5,}$/;
    let checkpassword = $('#Password').val();

    if (passwordregex.test(checkpassword)) {
        $('#password').css('border-color', 'lightgrey');
        $('#errpassword').hide();

    } else {
        $('#errpassword').show();
        $('#errpassword').html("password must contain min. 5 character and 1 unique symbol");
        isvalid = false;
    }
}
function validate() {
    var isValid = true;
    if ($('#FirstName').val().length < 2) {
        $('#ErrFirstName').show();
        $('#ErrFirstName').html("Enter valid first name");
        isValid = false;
    }
    else {
        $('#FirstName').css('border-color', 'lightgrey');
        $('#ErrFirstName').hide();
    }

    if ($('#LastName').val().length < 2) {
        $('#ErrLastName').show();
        $('#ErrLastName').html("Enter valid last name");
        isValid = false;
    }
    else {
        $('#LastName').css('border-color', 'lightgrey');
        $('#ErrLastName').hide();
    }

    if ($('#Code').val().length < 1) {
        $('#ErrCode').show();
        $('#ErrCode').html("Enter valid code");
        isValid = false;
    }
    else {
        $('#Code').css('border-color', 'lightgrey');
        $('#ErrCode').hide();
    }
    let emailregex = /^([_\-\.0-9a-zA-Z]+)@([_\-\.0-9a-zA-Z]+)\.([a-zA-Z]){2,7}$/;
    let checkEmail = $('#EmailID').val();

    if (emailregex.test(checkEmail)) {
        $('#EmailID').css('border-color', 'lightgrey');
        $('#ErrEmailID').hide();

        
    } else {
        $('#ErrEmailID').show();
        $('#ErrEmailID').html("Enter valid Email Id");
        isValid = false;
    }

    let mobileregex =
        /^[0-9]{10}$/;
    let checkMobile = $('#Mobile').val()

    if (mobileregex.test(checkMobile)) {
        $('#Mobile').css('border-color', 'lightgrey');
        $('#ErrMobile').hide();
    }
    else {
        $('#ErrMobile').show();
        $('#ErrMobile').html("Enter valid mobile");
        isValid = false;
    }

 
    let salaryregex =
        /^[0-9]{4,6}$/;
    let checkSalary = $('#Salary').val()

    if (salaryregex.test(checkSalary)) {
        $('#Salary').css('border-color', 'lightgrey');
        $('#ErrSalary').hide();
    }
    else {
        $('#ErrSalary').show();
        $('#ErrSalary').html("Enter valid salary");
        isValid = false;
    }

    return isValid;
}








