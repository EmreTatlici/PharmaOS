const { app, BrowserWindow } = require("electron");

const createWindow = () => {
  const win = new BrowserWindow({
    width: 1400,
    height: 900,
  });
  win.loadURL("http:" + "//localhost:5174");
};

app.whenReady().then(() => {
  createWindow();
});
