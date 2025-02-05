# **README File**

## **RetroSketch Tool**

RetroSketch tool for continuous measurement of emotions and presence in immersive experiences.

## **Introduction**

RetroSketch Tool is a new method for measuring emotions and presence in immersive experiences. It yields a continuous stream of subjective data about user experiences complementing existing methods such as experience sampling and physiological sensing. The VR experience is replayed both visually and audibly from the user’s perspective allowing the user to hear not only themselves but also the audio soundscape of their immersive experience. Users can identify ‘keypoints’ in their experience such as salient events with emotional consequences and sketch and plot the same over the duration of their experience. As a result, continuous data for users’ emotions and presence can be generated for each measure. In addition, users can also use textual annotations to provide context and sentiments associated with the keypoints. Therefore, the RetroSketch tool offers both quantitative and qualitative measures of users’ experience. Moreover, this tool offers several advantages over existing methods including limited observer bias over the experience itself, reduced social desirability etc. For further information about this tool and its use in comparison to other methods such as experience sampling, please refer to: [INSERT PAPER LINK]

## **Installation**

To install RetroSketch Tool, follow these steps:

1. Clone the repository: **`git clone https://github.com/revealcentre/retrosketch`**
2. Navigate to the project directory: **`cd retrosketch`**
3. Install dependencies: **`npm install`**
4. Build the project: **`npm run build`**
5. Start the project: **`npm start`**

You can modify the source code to fit your needs.

## **Usage**

To use RetroSketch Tool, follow these steps:

### **Set Up**

6. Import your desired video footage into the RetroSketch tool from the file directory that opens upon launch and press “Load”.
7. Once the video has loaded, you will see the graphical interface of the RetroSketch Tool. 

### **Researchers can give the following instructions to the participants:**
1. Here, the X-axis is representative of the footage timeline while the Y-axis is made up of the 10-point (0 least to 10 most) rating scale of each of the five emotions (Joy, Fear, Relaxation, Boredom, Presence).

#### **Play/Pause, Fast Forward and Rewind**

2. You can use the play and pause button to review the footage.
3. You can also use the red cursor along the cyan timeline bar to manually scroll through the footage if you would like. Please ensure the video is paused using the play/pause button once you have reached your desired timestamp. 
4. You can also fast forward or rewind the footage by 10 seconds each using the respective buttons placed on either side of the play/pause button.

#### **Video Speed**

5. RetroSketch Tool also offers a button to review footage and different speeds with a maximum of 2 times faster.

#### **Creating and Erasing Key Points**

6. When you are at your desired time point, such as a memorable moment in the footage, first ensure that the video is paused.
7. Next, switch to “Key point”  mode by tapping once on the “Line” button and place a point, rating each of your emotions for the time point.
8. You can now do this throughout the footage by either playing and pausing until you reach the next desired moment, fast forwarding or rewinding or manually scrolling through the footage.
9. If you make an error, or change your mind about any of the points, you can use the eraser tool to erase the point. To do so, simply tap on the eraser tool and then on the incorrect/misplaced keypoint.
10. To replace a keypoint after erasing it, first ensure that the timeline cursor is in line with the time point where you’d like to replace the point.
11. Next, ensuring that the “Key point” mode is on, place a point. Continue until you have created your desired number of time stamps throughout the entire footage.

#### **Creating Annotations**

12. Upon placing a point for each of the emotions for a time point, you will notice that the initial red colour has changed to green. This ensures that all points have been placed for the same time point. 
13. You will also notice a dialogue box pop open. Here you can annotate your keypoints by writing about your experience, such as describing the scene or why you felt how you felt rather than what you felt. 
14. Once you are happy with your text, you can press close. Please note that there is a minimum character input limit of [INSERT LIMIT] and as such you might need to populate the field more before you can close it.
15. You can also edit your annotations at any time by clicking on the relevant annotation on the blue timeline.
16. If the keypoints have not all turned green and the dialogue box has not popped upon, please check for any missing points or points not in line. This can sometimes happen if the footage was not paused prior to placing the points or if the timeline cursor was not aligned to the set of points when erasing and replacing a point.

#### **Connecting Key Points - Line Mode**

17. Once you have created your desired keypoints for 2 or more timestamps, you can switch to “Line” mode and connect each point. Please note that lines can either be created straight by joining two points or can have more variation, depending on what is the most suitable for you. 
18. Lines can only be drawn left to right. Please ensure that all lines are well connected and are not broken.

#### **Amending the Connecting Line**

19. If you make an error, or change your mind about how you have connected any of the points, you can use the eraser tool to erase the entire line or a section of it. To do so, simply tap on the eraser tool and then on the incorrect/misplaced part(s)of the line.
20. To redraw the line after erasing it, follow steps 17 and 18.

#### **Exporting the Graph Image**

21. Once you are happy with your work, please ask the researcher to take over again.
22. The researcher should move the timeline cursor to the extreme right and click on the “Export” button. 
23. If you are happy with the line and keypoint validation results, please press “Yes” to export a screenshot else press “No” and ask the participant to amend as needed.
24. Please note, we recommend ideally having at least 1 point per emotion every 5 minutes. 


## **Contributing**

If you'd like to contribute to RetroSketch Tool, here are some guidelines:

1. Fork the repository.
2. Create a new branch for your changes.
3. Make your changes.
4. Write tests to cover your changes.
5. Run the tests to ensure they pass.
6. Commit your changes.
7. Push your changes to your forked repository.
8. Submit a pull request.

## **License**

RetroSketch Tool is released under the AGPL-3.0 license. See the **[AGPL-3.0 license](https://github.com/revealcentre/retrosketch?tab=AGPL-3.0-1-ov-file)** file for details.

## **Authors and Acknowledgment**

RetroSketch Tool was created by REVEAL **(https://github.com/revealcentre)** and funded by the European Media and Immersion Lab (EMIL).

Additional contributors include:

- **[Contributor Name](https://github.com/contributor-name)**
- **[Another Contributor](https://github.com/another-contributor)**

Thank you to all the contributors for their hard work and dedication to the project.

## **Code of Conduct**

Please note that this project is released with a Contributor Code of Conduct. By participating in this project, you agree to abide by its terms. See the **[CODE_OF_CONDUCT.md](https://www.blackbox.ai/share/CODE_OF_CONDUCT.md)** file for more information.

## **FAQ**

**Q:** What is RetroSketch Tool?

**A:** RetroSketch Tool is a new method for continuous measurement of emotions and presence in immersive experiences.

**Q:** How do I install RetroSketch Tool?

**A:** Follow the installation steps in the README file.

**Q:** How do I use RetroSketch Tool?

**A:** Follow the usage steps in the README file.

**Q:** How do I contribute to RetroSketch Tool?

**A:** Follow the contributing guidelines in the README file.

**Q:** What license is RetroSketch Tool released under?

**A:** RetroSketch Tool is released under the AGPL-3.0 license. See the **[AGPL-3.0 license](https://github.com/revealcentre/retrosketch?tab=AGPL-3.0-1-ov-file)** file for details.

## **Changelog**

- **0.1.0:** Initial release
- **0.1.1:** Example-1 Fixed a bug in the build process
- **0.2.0:** Example-2 Added a new feature
- **0.2.1:** Example-3 Fixed a bug in the new feature

## **Contact**

If you have any questions or comments about RetroSketch Tool, please contact **REVEAL Centre at revealcentre@gmail.com**.

## **Conclusion**




