curl --silent --fail http://localhost:5000/api/mutation/resetdb --data '{}' --header "Content-Type: application/json"
curl --silent --fail http://localhost:5000/api/mutation/seed --data '{}' --header "Content-Type: application/json"

curl --silent --fail tutor-school-backend-demo.westeurope.azurecontainer.io:5000/api/mutation/resetdb --data '{}' --header "Content-Type: application/json"
curl --silent --fail tutor-school-backend-demo.westeurope.azurecontainer.io:5000/api/mutation/seed --data '{}' --header "Content-Type: application/json"

tutor-school-backend-demo.westeurope.azurecontainer.io