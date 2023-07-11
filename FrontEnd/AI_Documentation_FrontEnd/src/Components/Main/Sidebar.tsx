import React from "react";
import {
  Box,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import InboxIcon from "@mui/icons-material/MoveToInbox";
import MailIcon from "@mui/icons-material/Mail";
import CloudUploadIcon from "@mui/icons-material/CloudUpload";

const sidebarItems = ["Inbox", "Starred", "Send email", "Drafts"];
const Sidebar: React.FC = () => {
  return (
    <Drawer variant="permanent" anchor="left">
      <Box display="flex" flexDirection="column" height="100%">
        <List>
          {sidebarItems.map((text, index) => (
            <ListItem button key={text}>
              <ListItemIcon>
                {index % 2 === 0 ? <InboxIcon /> : <MailIcon />}
              </ListItemIcon>
              <ListItemText primary={text} />
            </ListItem>
          ))}
        </List>
        <Box marginTop="auto">
          <List>
            <ListItem button key="Upload">
              <ListItemIcon>
                <CloudUploadIcon />
              </ListItemIcon>
              <ListItemText primary="Upload" />
            </ListItem>
          </List>
        </Box>
      </Box>
    </Drawer>
  );
};

export default Sidebar;

