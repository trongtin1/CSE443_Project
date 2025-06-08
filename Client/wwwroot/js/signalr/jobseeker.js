// JobSeeker SignalR connections
"use strict";

document.addEventListener("DOMContentLoaded", function () {
  // Get the current JobSeeker ID
  const jobSeekerId =
    document.getElementById("current-jobseeker-id")?.value ||
    localStorage.getItem("jobSeekerId");

  if (!jobSeekerId) {
    console.log("No JobSeeker ID found, skipping JobSeeker SignalR setup");
    return;
  }

  console.log(`Setting up JobSeeker SignalR for JobSeeker ID: ${jobSeekerId}`);

  // Create connection to JobHub
  const jobHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/jobHub")
    .withAutomaticReconnect()
    .build();

  // Handle application status changes
  jobHubConnection.on("ApplicationStatusChanged", (applicationId, status) => {
    console.log(`Application ${applicationId} status changed to ${status}`);

    let title, message, link;

    // Format message based on status
    switch (status) {
      case "Shortlisted":
        title = "Application Shortlisted";
        message = `Your application #${applicationId} has been shortlisted for the next round.`;
        link = `/JobSeeker/Candidates`;
        break;
      case "Interviewed":
        title = "Interview Scheduled";
        message = `Your application #${applicationId} has been scheduled for an interview. Please check the details.`;
        link = `/JobSeeker/Candidates`;
        break;
      case "Hired":
        title = "Congratulations! You have been hired";
        message = `Your application #${applicationId} has been accepted. You have been hired!`;
        link = `/JobSeeker/Candidates`;
        break;
      case "Rejected":
        title = "Application Rejected";
        message = `We're sorry, your application #${applicationId} was not accepted.`;
        link = `/JobSeeker/Candidates`;
        break;
      default:
        title = "Application Update";
        message = `Your application #${applicationId} has been updated to "${status}".`;
        link = `/JobSeeker/Candidates`;
    }

    // Add notification using the global function from _NotificationsPartial.cshtml
    if (window.addNotification) {
      window.addNotification(title, message, "application", link);
    } else {
      console.error("addNotification function not available");
      // Fallback to alert if notification system isn't loaded
      alert(`${title}: ${message}`);
    }
  });

  // Join the JobSeeker group
  jobHubConnection
    .start()
    .then(() => {
      console.log("JobHub connection established for JobSeeker");

      // Join the JobSeeker group using the JobSeeker ID
      return jobHubConnection.invoke("JoinGroup", `jobseeker_${jobSeekerId}`);
    })
    .then(() => {
      console.log(`Joined JobSeeker group: jobseeker_${jobSeekerId}`);
    })
    .catch((err) => {
      console.error("Error with JobHub connection for JobSeeker:", err);
    });
});
