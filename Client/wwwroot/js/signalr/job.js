"use strict";

// Create connection to the job hub
const jobConnection = new signalR.HubConnectionBuilder()
  .withUrl("/jobHub")
  .withAutomaticReconnect()
  .build();

// Start the connection
function startJobConnection() {
  jobConnection
    .start()
    .then(() => {
      console.log("Job hub connection established");

      // Join job categories that the user is interested in
      // This should be populated based on user preferences
      const userCategories = getUserPreferredCategories();
      if (userCategories && userCategories.length > 0) {
        userCategories.forEach((category) => {
          jobConnection
            .invoke("JoinJobCategory", category)
            .catch((err) =>
              console.error(`Error joining category ${category}: ${err}`)
            );
        });
      }
    })
    .catch((err) => {
      console.error("Error while establishing job connection: " + err);
      // Retry connection after 5 seconds
      setTimeout(startJobConnection, 5000);
    });
}

// Handle connection closed
jobConnection.onclose((error) => {
  console.log("Job hub connection closed");
  if (error) {
    console.error("Connection closed due to error: " + error);
  }
  // Attempt to restart the connection
  setTimeout(startJobConnection, 5000);
});

// Helper function to get user's preferred job categories
function getUserPreferredCategories() {
  // This should be implemented to get categories from user preferences
  // For now, return an empty array or hardcoded values for testing
  return [];
}

// Register handlers for job-related events
jobConnection.on("ReceiveNewJob", (jobId, jobTitle, company) => {
  showNewJobNotification(jobId, jobTitle, company);
});

jobConnection.on("ApplicationStatusChanged", (applicationId, status) => {
  showApplicationStatusNotification(applicationId, status);
});

jobConnection.on("NewApplication", (jobId, applicantName) => {
  showNewApplicationNotification(jobId, applicantName);
});

jobConnection.on("NewJobInCategory", (category, jobId, jobTitle) => {
  showNewJobInCategoryNotification(category, jobId, jobTitle);
});

// Helper functions to display job-related notifications
function showNewJobNotification(jobId, jobTitle, company) {
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification job-notification";
  notificationElement.innerHTML = `
        <div class="notification-content">
            <strong>New Job Posted:</strong> ${jobTitle} at ${company}
            <a href="/Job/Details/${jobId}" class="notification-link">View Job</a>
        </div>
    `;

  showNotification(notificationElement);
}

function showApplicationStatusNotification(applicationId, status) {
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification application-notification";
  notificationElement.innerHTML = `
        <div class="notification-content">
            <strong>Application Update:</strong> Your application status changed to "${status}"
            <a href="/Application/Details/${applicationId}" class="notification-link">View Application</a>
        </div>
    `;

  showNotification(notificationElement);
}

function showNewApplicationNotification(jobId, applicantName) {
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification employer-notification";
  notificationElement.innerHTML = `
        <div class="notification-content">
            <strong>New Application:</strong> ${applicantName} applied to your job
            <a href="/Employer/JobApplications/${jobId}" class="notification-link">View Applications</a>
        </div>
    `;

  showNotification(notificationElement);
}

function showNewJobInCategoryNotification(category, jobId, jobTitle) {
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification category-notification";
  notificationElement.innerHTML = `
        <div class="notification-content">
            <strong>New ${category} Job:</strong> ${jobTitle}
            <a href="/Job/Details/${jobId}" class="notification-link">View Job</a>
        </div>
    `;

  showNotification(notificationElement);
}

// Generic function to show notification
function showNotification(notificationElement) {
  const notificationContainer = document.getElementById(
    "notification-container"
  );
  if (notificationContainer) {
    notificationContainer.appendChild(notificationElement);

    // Auto remove after 5 seconds
    setTimeout(() => {
      notificationElement.classList.add("fade-out");
      setTimeout(() => {
        notificationContainer.removeChild(notificationElement);
      }, 500);
    }, 5000);
  }
}

// Start the connection when the document is ready
document.addEventListener("DOMContentLoaded", () => {
  startJobConnection();
});
