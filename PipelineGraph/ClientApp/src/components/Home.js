import React, { Component } from 'react';
import { Redirect } from "react-router-dom";
import { Button, NavLink  } from 'reactstrap';


export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>A pipeline dashboard for multi repository builds with GitHub Actions</h1>
        <p>It is very common to use time during SW development in order to keep an eye on your builds.
        A good overview of the build chain is a true help to developers. Build systems, such as TeamCity or Jenkins, have solved this problem
        by introducing the Dependency Graph View (DGV) (see <a href="https://www.jetbrains.com/help/teamcity/build-dependencies-setup.html#Basics">DGV</a>) .
        </p><p>More and more develpers/organizations either start using Github for their projects or migrate the existing ones from the on-premise setup.
        Github actions gives a very nice (and similar to Jenkins/TeamCity) graphical overview of the jobs in the workflow during the build. Still many
        complex setups coming from big organizations combine several repositories into a build and a good overview of the build is missing.
        </p><p>While there are already attempts to solve this problem from the community side (see <a href="https://medium.com/@sitapati/complex-multi-repo-builds-with-github-actions-and-camunda-cloud-fa8e4c7abd26">Complex multi-repo builds with GitHub Actions and Camunda Cloud</a>)
        we propose a similar, but also more lightweight solution, that is deployed on DigitalOcean and is powerred by Mermaid diagram viewer.
        </p>
        <p>The pipeline dashboard is an event based system. There are two special even types: initialization and status updates to the jobs in the dashboard. The complete list of examples for each update event and initialization is given below.</p>
        <img src="architecture.svg" id="architecture" /><br/>
        <p>The implemented solution is not limited to Github actions and can be deployed on any platform that either has DotNet5 or Docker installed.</p>
       <img src="dashboard.svg" id="dashboard" /><br/>
        If you would like to initialize the pipeline dashboard with an example:
          <pre class="bash">
          curl --header "Content-Type: application/json"  \<br/>
        --request POST --data '[["doctl","sample-golang"],["sample-dockerfile","sample-gatsby"],["doctl","sample-gatsby"],["godo","sample-gatsby"],["go-workers2","sample-gatsby"],["pynetbox","hacktoberfest"],["terraform-provider-digitalocean","grafana"],["pynetbox","doctl"],["sample-golang","godo"],["hacktoberfest","godo"],["droplet_kit","godo"],["sample-laravel-api","openapi"],["sample-gatsby","nginxconfig.io"],["go-libvirt","terraform-provider-digitalocean"],["go-libvirt","droplet_kit"],["sample-laravel-api","go-workers2"],["clusterlint","go-workers2"],["openapi","clusterlint"],["clusterlint","clusterlint"]]' \<br/>
        https://your-hostname/pipelinegraph/init
         </pre>
         You will get the session ID back, that you will use in both viewing the dashboard and updating the status' of the jobs.<br/>
         To access the dashboard, visit https://your-hostname/graph/session-id<br/>
        A job has been started:
         <pre class="bash">
         curl --header "Content-Type: application/json"   --request POST --data '["hacktoberfest","running"]' https://your-hostname/pipelinegraph/updatestatus/session-id
         </pre>
         A job finished with success:
         <pre class="bash">
         curl --header "Content-Type: application/json"   --request POST --data '["doctl","finishedsuccess"]' https://your-hostname/pipelinegraph/updatestatus/session-id
         </pre>
         A job failed:
         <pre class="bash">
         curl --header "Content-Type: application/json"   --request POST --data '["terraform-provider-digitalocean","finishederror"]' https://your-hostname/pipelinegraph/updatestatus/session-id
         </pre>
      </div>
    );
  }

}
