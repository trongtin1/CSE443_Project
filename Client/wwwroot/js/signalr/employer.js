"use strict";

// Khởi tạo kết nối SignalR cho employer
document.addEventListener("DOMContentLoaded", function () {
  // Kiểm tra xem có phải là employer không và chưa có kết nối
  const employerId =
    document.getElementById("current-employer-id")?.value ||
    localStorage.getItem("employerId");

  // Only initialize if we're an employer and window.employerConnection doesn't exist
  if (employerId && !window.employerConnection) {
    console.log(
      `Initializing employer connection for employer ID: ${employerId}`
    );

    // Lưu employerId vào localStorage để sử dụng sau này
    localStorage.setItem("employerId", employerId);

    // Tạo kết nối đến JobHub
    window.employerConnection = new signalR.HubConnectionBuilder()
      .withUrl("/jobHub")
      .withAutomaticReconnect()
      .build();

    // Xử lý sự kiện nhận thông báo ứng tuyển mới
    window.employerConnection.on("NewApplication", (jobId, applicantName) => {
      console.log(
        `Received new application notification: ${applicantName} applied for job ${jobId}`
      );

      // Use the global notification function to show the notification
      if (window.showNewApplicationNotification) {
        window.showNewApplicationNotification(jobId, applicantName);
      }
    });

    // Xử lý sự kiện debug
    window.employerConnection.on("DebugNotification", (message) => {
      console.log(`Debug notification: ${message}`);
    });

    // Khởi động kết nối
    window.employerConnection
      .start()
      .then(() => {
        console.log("Employer connection established");

        // Đăng ký tham gia nhóm employer
        return window.employerConnection.invoke("JoinAsEmployer", employerId);
      })
      .then(() => {
        console.log(`Successfully joined as Employer ${employerId}`);
      })
      .catch((err) => {
        console.error("Error establishing employer connection: " + err);

        // Thử kết nối lại sau 5 giây
        setTimeout(() => {
          console.log("Attempting to reconnect...");
          window.employerConnection
            .start()
            .then(() => {
              console.log("Reconnected successfully");
              return window.employerConnection.invoke("JoinAsEmployer", employerId);
            })
            .then(() => {
              console.log(
                `Successfully joined as Employer ${employerId} after reconnection`
              );
            })
            .catch((err) => {
              console.error("Failed to reconnect: " + err);
            });
        }, 5000);
      });

    // Xử lý sự kiện đóng kết nối
    window.employerConnection.onclose((error) => {
      console.log("Employer connection closed");
      if (error) {
        console.error("Connection closed due to error: " + error);
      }

      // Thử kết nối lại sau 5 giây
      setTimeout(() => {
        window.employerConnection
          .start()
          .then(() => {
            console.log("Reconnected after connection closed");
            return window.employerConnection.invoke("JoinAsEmployer", employerId);
          })
          .catch((err) => {
            console.error(
              "Failed to reconnect after connection closed: " + err
            );
          });
      }, 5000);
    });
  }
});
