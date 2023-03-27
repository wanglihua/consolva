package com.huadu.shunda.data.http.login;

import com.google.gson.annotations.SerializedName;

public class LoginResponseData {
    @SerializedName("userId")
    private int userId;

    @SerializedName("userName")
    private String userName;

    @SerializedName("canOrderPhotoSignIn")
    private boolean canOrderPhotoSignIn;

    @SerializedName("canOrderPhotoCheckIn")
    private boolean canOrderPhotoCheckIn;

    @SerializedName("canOrderPhotoIssue")
    private boolean canOrderPhotoIssue;

    @SerializedName("canOrderPhotoCnDamage")
    private boolean canOrderPhotoCnDamage;

    @SerializedName("canOrderPhotoReturn")
    private boolean canOrderPhotoReturn;

    @SerializedName("canOrderPhotoShip")
    private boolean canOrderPhotoShip;

    @SerializedName("canOrderPhotoAuDamage")
    private boolean canOrderPhotoAuDamage;

    @SerializedName("canOrderPhotoDeliver")
    private boolean canOrderPhotoDeliver;

    @SerializedName("canOrderPhoto")
    private boolean canOrderPhoto;

    public int getUserId() {
        return userId;
    }

    public void setUserId(int userId) {
        this.userId = userId;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public boolean getCanOrderPhotoSignIn() {
        return canOrderPhotoSignIn;
    }

    public void setCanOrderPhotoSignIn(boolean canOrderPhotoSignIn) {
        this.canOrderPhotoSignIn = canOrderPhotoSignIn;
    }

    public boolean getCanOrderPhotoCheckIn() {
        return canOrderPhotoCheckIn;
    }

    public void setCanOrderPhotoCheckIn(boolean canOrderPhotoCheckIn) {
        this.canOrderPhotoCheckIn = canOrderPhotoCheckIn;
    }

    public boolean getCanOrderPhotoIssue() {
        return canOrderPhotoIssue;
    }

    public void setCanOrderPhotoIssue(boolean canOrderPhotoIssue) {
        this.canOrderPhotoIssue = canOrderPhotoIssue;
    }

    public boolean getCanOrderPhotoCnDamage() {
        return canOrderPhotoCnDamage;
    }

    public void setCanOrderPhotoCnDamage(boolean canOrderPhotoCnDamage) {
        this.canOrderPhotoCnDamage = canOrderPhotoCnDamage;
    }

    public boolean getCanOrderPhotoReturn() {
        return canOrderPhotoReturn;
    }

    public void setCanOrderPhotoReturn(boolean canOrderPhotoReturn) {
        this.canOrderPhotoReturn = canOrderPhotoReturn;
    }

    public boolean getCanOrderPhotoShip() {
        return canOrderPhotoShip;
    }

    public void setCanOrderPhotoShip(boolean canOrderPhotoShip) {
        this.canOrderPhotoShip = canOrderPhotoShip;
    }

    public boolean getCanOrderPhotoAuDamage() {
        return canOrderPhotoAuDamage;
    }

    public void setCanOrderPhotoAuDamage(boolean canOrderPhotoAuDamage) {
        this.canOrderPhotoAuDamage = canOrderPhotoAuDamage;
    }

    public boolean getCanOrderPhotoDeliver() {
        return canOrderPhotoDeliver;
    }

    public void setCanOrderPhotoDeliver(boolean canOrderPhotoDeliver) {
        this.canOrderPhotoDeliver = canOrderPhotoDeliver;
    }

    public boolean getCanOrderPhoto() {
        return canOrderPhoto;
    }

    public void setCanOrderPhoto(boolean canOrderPhoto) {
        this.canOrderPhoto = canOrderPhoto;
    }
}
