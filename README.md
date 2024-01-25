# Restaurant-Manager_Webpage
This project was created as part of an engineering thesis.   

<div align="center">
  <table style="width:100%">
    <tr align="center">
      <td>Package name</td>
      <td>Package version</td>
    </tr>
    <tr>
      <td>Microsoft.AspNetCore.Authentication.JwtBearer</td>
      <td align="center">6.0.15</td>
    </tr>
   <tr>
      <td>Newtonsoft.Json</td>
      <td align="center">13.0.3</td>
    </tr>
  </table>
</div>

# How to run webpage
1) You have to setup <a href="https://github.com/ZiumC/Restaurant-Manager_REST-API" rel="nofollow">REST API</a>.
2) You need to fillup following application settings:
``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ApplicationSettings": {
    "BasicBonus": "150",
    "DataValidation": {
      "LoginRegex": "^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]+$",
      "EmailRegex": "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$",
      "PeselRegex": "^[0-9]{11}$"
    },
    "ComplaintStatus": {
      "New": "NEW",
      "Accepted": "ACCEPTED",
      "Pending": "PENDING",
      "Rejected": "REJECTED"
    },
    "ComplaintActions": {
      "Consider": "consider",
      "Accept": "accept",
      "Reject": "reject"
    },
    "JwtSettings": {
      //This field should be as same as well REST API appsettings.json
      "Issuer": "https://localhost:7042",
      //This field is webpage host address. Also should be as same as well REST API appsettings.json
      "Audience": "https://localhost:7245",
      "SecretSignatureKey": "",
      "AccessTokenValidity": "2",
      "CookieSettings": {
        "CookieName": "AccessToken"
      }
    },
    "AdministrativeRoles": {
      "Owner": "Owner",
      "Supervisor": "Chef",
      "OwnerStatusYes": "Y",
      "OwnerStatusNo": "N"
    },
    "UserSettings": {
      "Login": {
        "MinLength": "3",
        "MaxLength": "50"
      },
      "Password": {
        "MinLength": "3",
        "MaxLength": "50"
      },
      "CookieSettings": {
        "Common": {
          "FieldName": "name",
          "RoleName": "role"
        },
        "Client": {
          "IdName": "ClientId"
        },
        "Supervisor": {
          "IdName": "EmpId"
        }
      }
    }
  },
  "Endpoints": {
    //REST API address url
    "BaseHost": "https://localhost:7042",
    "Controller": {
      "Clients": "/api/Clients",
      "Users": "/api/Users",
      "Complaints": "/api/manage/Complaints",
      "Employees": "/api/manage/Employees",
      "Restaurants": "/api/manage/Restaurants"
    },
    "Paths": {
      "Login": "/login",
      "Register": "/register",
      "Reservation": "/reservation",
      "Restaurants": "/restaurants",
      "Restaurant": "/restaurant",
      "Confirm": "/confirm",
      "Cancel": "/cancel",
      "Complaint": "/complaint",
      "Certificate": "/certificate",
      "Rate": "/rate?grade=",
      "Dishes": "/dishes",
      "Dish": "/dish",
      "Employee": "/employee",
      "Types": "/types",
      "Type": "/type",
      "Stats": "/stats"

    }
  }
}
```
3. Just start appliacion from IDE MS Visual Studio Community 2022

# Important notes
1. ```Issuer``` - value of this field should be equal in ```REST API/appsettings.json``` and ```Webpage/appsettings.json```. If values will be different JWT won't be accepted in REST API nor webpage.
2. ```Audience``` - value of this field should be equal in ```REST API/appsettings.json``` and ```Webpage/appsettings.json```. If values will be different JWT won't be accepted in REST API nor webpage.
3. ```BaseHost``` - bear in mind it is REST API url
