package com.guys.go.love.tea;

import android.app.Service;
import android.content.Intent;
import android.os.Binder;
import android.os.IBinder;
import android.os.Parcel;
import android.util.Log;

import java.util.Calendar;
import java.util.Date;
import java.util.Set;
import java.util.Timer;
import java.util.TimerTask;

public class MyPushService extends Service {
	private final static String TAG = "PiggyPushService";
	public static String ENABLE_ALL = "enableAll";
	public static String DISABLE_ALL = "disableAll";
	private TimerTask task = null;
	private Timer timer = null;
	public final static String ACTION="cn.com.action.MyAction";

    private final IBinder binder = new MyPushBinder();
	private MessageNotification mMessageNotification;
	public static final int TIME_NOTICE = 100;
	
    public class MyPushBinder extends Binder {
    	public MyPushService getService() {
            return MyPushService.this;
        }  
    	
        public boolean onTransact(int code , Parcel data , Parcel reply , int flags ) {
            return handleTransactions( code , data , reply);  
        }  
    }  
    
    
	@Override
	public IBinder onBind(Intent arg0) {
		return binder;
	}

	@Override
	public void onCreate() {
		super.onCreate();
		checkServiceStatus();
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {
		mMessageNotification=MessageNotification.getInstance(this);
		if(intent == null)
		{
			return super.onStartCommand(intent, flags, startId);
		}
		boolean startBySys = intent.getBooleanExtra("sys", false);
		if(!startBySys){
			mMessageNotification.disableAll();
		}
		doTask();
		return super.onStartCommand(intent, flags, startId);
	}

	
	private void doTaskOne(final String key){
		Date startDate = new Date(mMessageNotification.getNotificationTime(key));
		Date twoDaysAgo = new Date(System.currentTimeMillis() - 2* 24*3600L);
		Date tenDaysLater = new Date(System.currentTimeMillis() + 10* 24*3600L);
		if(startDate== null || startDate.before(twoDaysAgo) || startDate.after(tenDaysLater))
		{
			return;
		}
		task = new TimerTask() {
			@Override
			public void run() {
				try{
						Date innerStartDate = new Date(mMessageNotification.getNotificationTime(key));
						Long n= System.currentTimeMillis();
						Long s=innerStartDate.getTime();
						Log.w(new Date(n) +"", key+"," + new Date(s));
						if(n > s - 3000){
							doTaskGetMessage(key);	
						}
					} catch (Exception e) {
						e.printStackTrace();
					}
			}
		};
		timer.schedule(task, startDate); 
	}
	private void doTask() {
		timer = new Timer();
		Set<String> keyset = mMessageNotification.getAllKeys();
		for(String key: keyset){
			doTaskOne(key);
		}

		Calendar c = Calendar.getInstance();
		c.set(Calendar.MINUTE, 0);
		c.set(Calendar.SECOND, 0);
		c.set(Calendar.MILLISECOND, 0);
		
		c.set(Calendar.HOUR_OF_DAY, 12);
		Date dt1 = c.getTime();
		c.set(Calendar.HOUR_OF_DAY, 18);
		Date dt2 = c.getTime();
		c.set(Calendar.HOUR_OF_DAY, 21);
		Date dt3 = c.getTime();
		c.set(Calendar.HOUR_OF_DAY, 22);
		Date dt4 = c.getTime();

		if(dt1.before(new Date()))
		{
			dt1 = new Date(dt1.getTime() + 24*3600*1000L);
		}
		if(dt2.before(new Date()))
		{
			dt2 = new Date(dt2.getTime() + 24*3600*1000L);
		}
		if(dt3.before(new Date()))
		{
			dt3 = new Date(dt3.getTime() + 24*3600*1000L);
		}
		if(dt4.before(new Date()))
		{
			dt4 = new Date(dt4.getTime() + 24*3600*1000L);
		}

		TimerTask tt1 = showTT(1, dt1, "�����������������ǵ���һ������ͣ�");
		TimerTask tt2 = showTT(2, dt2, "�����������������ǵ���һ������ͣ�");
		TimerTask tt3 = showTT(3, dt3, "�����������������ǵ���һ�𹲽���ҹ��");
		TimerTask tt4 = showTT(3, dt4, "ʤ���Ĺ�ʵ���ˣ����������������ˣ�������ȡ��ʯ��ƻ���Ұɣ�");

		timer.scheduleAtFixedRate(tt1, dt1, 24*3600*1000L);
		timer.scheduleAtFixedRate(tt2, dt2, 24*3600*1000L);
		timer.scheduleAtFixedRate(tt3, dt3, 24*3600*1000L);
		timer.scheduleAtFixedRate(tt4, dt4, 24*3600*1000L);
	}

	private TimerTask showTT(final int type, final Date dt, final String cnt)
	{
		return new TimerTask() {
			@Override
			public void run() {
				try{
					int id = (int)((dt.getTime()/24/3600/1000) * 10 + type);
					mMessageNotification.sendNotification(id, cnt);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		};
	}
	

    private boolean handleTransactions(int code , Parcel data , Parcel reply){
    	if(code == TIME_NOTICE){
    		String key = data.readString();
    		long time = data.readLong();
    		String title = data.readString();
    		String content = data.readString();

    		if(ENABLE_ALL.equals(key)){
    			mMessageNotification.enableAll();
    			return true;
    		}
    		if(DISABLE_ALL.endsWith(key)){
    			mMessageNotification.disableAll();
    			return true;
    		}
    		if(title == null || title.length() == 0){
    			mMessageNotification.resetNotificationStatus(key);
    			return true;
    		}
	    	mMessageNotification.saveNotificationInfo(key, time, title, content);
	    	doTaskOne(key);
    	}
    	return true;
    }
	
	private void doTaskGetMessage(String key){
		Log.w(TAG, "------------���ݶ�ʱ����ʱ���������Ϣ��ʾ!");
		//Ĭ����1000 ����״̬ �ı�����
		int status = mMessageNotification.getNotificationStatus(key);
		int inGame= mMessageNotification.checkIsInGame();
		Log.w(TAG, "ingame = " + inGame);
		if(status == MessageNotification.READY && inGame == MessageNotification.DONE){
			Log.w(TAG, TAG+"��ʼִ�������� ");
			mMessageNotification.sendNotification(key);
		}
		else{
			Log.w(TAG, TAG+"ִ����Ч ");
		}
	}
	
	
	
	
	
	private void checkServiceStatus() {
		new Thread() {
			@Override
			public void run() {
				super.run();
				try {
					String filePath = "com.example.clearicon.MyPushService";
					boolean flag = MyUtil.isServiceRunning(MyPushService.this,filePath);
					if (flag == false) {
						MyPushService.this.startService(new Intent(MyPushService.this, MyPushService.class));
					}
				} catch (Exception e) {

				}
			}
		}.start();
		
	}
	


	@Override
	public void onDestroy() {
		// TODO Auto-generated method stub
		super.onDestroy();
	}

}
