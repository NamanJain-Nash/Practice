## Types of Function app
<p>Their are mainly 2 types of function that is a azure function that is: </p>
<ul>
<li>Azure Http Trigger Function in Post </li>
<li>Azure Event Hub Trigger</li>
</ul>

## Azure Http Trigger Function
<p>Both are present in FunctionApp1 folder and in this main function is done in HttpTriggerCosmoDirect.cs file</p>
<p>These have two function</p>
<p> 1) It  directly take the data and then safe it into the cosmo db and error to storage queue Function name HttpTriggerForMessage</p>
<p>2) The second is one that call and pass the data into the event grid</p>

## Azure Event Hub Trigger
<p>This is the function that is called by the event hub as a reciver and also do the handling of saving the data to cosmos db and error to storage queue present in folder EventHub</p>

## Services
<p>The main are the services that are their to make the process be dependent and be reusable in nature and make the handling be done </p>
<p>Services that are being used are</p>
<ul>
<li>Cosmos Db Service this is the one that take the data and saves that data into the desired cosmos db of the connection that is their</li>
<li>Event hub Sevrice this is used to send the data to the event hub function</li>
<li>Making of the handling of the stroge queue</li>
</ul>